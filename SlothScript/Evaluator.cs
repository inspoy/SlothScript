using System;
using System.Collections.Generic;
using SlothScript.AST;

namespace SlothScript
{
    internal static class Evaluator
    {
        /// <summary>
        /// 对节点进行求值
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        public static EvalValue Eval(Environment env, AstNode ast)
        {
            var result = new EvalValue();
            if (ast is AstProg)
            {
                result = CalcProg(env, ast as AstProg);
            }
            else if (ast is AstExpression)
            {
                result = CalcExpression(env, ast as AstExpression);
            }
            else if (ast is AstIf)
            {
                result = CalcIf(env, ast as AstIf);
            }
            else if (ast is AstWhile)
            {
                result = CalcWhile(env, ast as AstWhile);
            }
            else if (ast is AstReturn)
            {
                result = CalcReturn(env, ast as AstReturn);
            }
            else if (ast is AstStringLiteral)
            {
                result = CalcString(env, ast as AstStringLiteral);
            }
            else if (ast is AstNumberLiteral)
            {
                result = CalcNumber(env, ast as AstNumberLiteral);
            }
            else if (ast is AstIdentifier)
            {
                result = CalcIdentifier(env, ast as AstIdentifier);
            }
            else
            {
                throw new RunTimeException("非法节点类型: " + ast.GetType().ToString(), ast);
            }
            Utils.LogDebug("[R] 正在计算[{0}]{1}的值，结果为：{2}", ast.GetType(), ast, result);
            return result;
        }

        private static EvalValue CalcProg(Environment env, AstProg ast)
        {
            var ret = new EvalValue();
            foreach (var item in ast)
            {
                ret = item.Eval(env);
            }
            if (env.Get("return") != null)
            {
                return env.Get("return");
            }
            return ret;
        }

        private static EvalValue CalcExpression(Environment env, AstExpression ast)
        {
            // 先计算右值
            var right = new EvalValue();
            // 可能的运算符 == <= >= && || + - * / < > %
            // && ||
            // >= <= == > <
            // + -
            // * / %
            try
            {
                Stack<AstPun> ops = new Stack<AstPun>();
                Queue<AstNode> output = new Queue<AstNode>();
                // 转为后缀表达式
                foreach (var item in ast)
                {
                    if (item is AstFactor || item is AstExpression)
                    {
                        output.Enqueue(item);
                    }
                    else if (item is AstPun)
                    {
                        while (ops.Count > 0 && ops.Peek() >= (item as AstPun))
                        {
                            output.Enqueue(ops.Pop());
                        }
                        ops.Push(item as AstPun);
                    }
                    else
                    {
                        throw new RunTimeException("非法表达式项", item);
                    }
                }
                while (ops.Count > 0)
                {
                    output.Enqueue(ops.Pop());
                }
                // 后缀表达式求值
                Stack<EvalValue> result = new Stack<EvalValue>();
                while (output.Count > 0)
                {
                    var item = output.Dequeue();
                    if (item is AstFactor || item is AstExpression)
                    {
                        result.Push(item.Eval(env));
                    }
                    else if (item is AstPun)
                    {
                        var val2 = result.Pop();
                        var val1 = result.Pop();
                        EvalValue res;
                        string op = (item as AstPun).token.ToString();
                        if (op == "+")
                        {
                            res = val1 + val2;
                        }
                        else if (op == "-")
                        {
                            res = val1 - val2;
                        }
                        else if (op == "*")
                        {
                            res = val1 * val2;
                        }
                        else if (op == "/")
                        {
                            res = val1 / val2;
                        }
                        else if (op == "%")
                        {
                            res = val1 % val2;
                        }
                        else if (op == "<")
                        {
                            res = val1 < val2;
                        }
                        else if (op == ">")
                        {
                            res = val1 > val2;
                        }
                        else if (op == "==")
                        {
                            res =
                                (val1.type == EvalValueType.INT && val2.type == EvalValueType.INT
                                && val1.intVal == val2.intVal) ||
                                (val1.type == EvalValueType.STRING && val2.type == EvalValueType.STRING
                                && val1.strVal == val2.strVal) ?
                                EvalValue.TRUE : EvalValue.FALSE;
                        }
                        else if (op == "<=")
                        {
                            res = val1 <= val2;
                        }
                        else if (op == ">=")
                        {
                            res = val1 >= val2;
                        }
                        else if (op == "&&")
                        {
                            res = (val1 > EvalValue.ZERO).intVal > 0 && (val2 > EvalValue.ZERO).intVal > 0 ?
                                EvalValue.TRUE : EvalValue.FALSE;
                        }
                        else if (op == "||")
                        {
                            res = (val1 > EvalValue.ZERO).intVal > 0 || (val2 > EvalValue.ZERO).intVal > 0
                                ? EvalValue.TRUE : EvalValue.FALSE;
                        }
                        else
                        {
                            throw new RunTimeException("非法运算符" + op);
                        }
                        result.Push(res);
                    }
                    else
                    {
                        throw new RunTimeException("非法表达式项", item);
                    }
                }
                if (result.Count != 1)
                {
                    throw new RunTimeException("表达式结果不正确");
                }
                right = result.Pop();
            }
            catch (Exception e)
            {
                throw new RunTimeException("计算表达式的值时出错:" + e.Message, ast);
            }

            if (ast.isAssignExpression)
            {
                // 赋值语句
                env.Set(ast.leftFactor, right);
            }
            return right;
        }

        private static EvalValue CalcIf(Environment env, AstIf ast)
        {
            // 返回值为执行的最后一条语句或表达式的值
            var ret = new EvalValue();
            var cond = ast.condition.Eval(env);
            if (cond.type == EvalValueType.STRING)
            {
                throw new RunTimeException("条件表达式不能为字符串", ast.condition);
            }
            else if (cond.type == EvalValueType.INT && cond.intVal > 0)
            {
                foreach (var item in ast)
                {
                    ret = item.Eval(env);
                }
            }
            else if (ast.elseBlock != null)
            {
                // 有else块
                foreach (var item in ast.elseBlock)
                {
                    ret = item.Eval(env);
                }
            }

            return ret;
        }

        private static EvalValue CalcWhile(Environment env, AstWhile ast)
        {
            // 返回值为最后一条语句或表达式的值
            var ret = new EvalValue();
            while (true)
            {
                var cond = ast.condition.Eval(env);
                if (cond.type == EvalValueType.INT && cond.intVal > 0)
                {
                    // 当条件满足时执行代码块中的每一条子语句
                    foreach (var item in ast)
                    {
                        ret = item.Eval(env);
                    }
                }
                else if (cond.type == EvalValueType.STRING)
                {
                    throw new RunTimeException("条件表达式不能为字符串", ast.condition);
                }
                else
                {
                    break;
                }
            }
            return ret;
        }

        private static EvalValue CalcReturn(Environment env, AstReturn ast)
        {
            var ret = ast.result.Eval(env);
            // TODO: 中断执行
            //ret.hasReturned = true;
            env.Set("return", ret);
            return ret;
        }

        private static EvalValue CalcString(Environment env, AstStringLiteral ast)
        {
            return new EvalValue(ast.str);
        }

        private static EvalValue CalcNumber(Environment env, AstNumberLiteral ast)
        {
            return new EvalValue(ast.value);
        }

        private static EvalValue CalcIdentifier(Environment env, AstIdentifier ast)
        {
            var val = env.Get(ast.name);
            if (val == null)
            {
                throw new RunTimeException("未定义的标识符", ast);
            }
            return val;
        }
    }
}
