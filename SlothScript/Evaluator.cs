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
        public static EvalValue Eval(IEnvironment env, AstNode ast)
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
            else if (ast is AstFuncDef)
            {
                env.GetMainEnv().SetFuncDef(ast as AstFuncDef);
                result = EvalValue.ZERO; // 函数定义，不返回任何有意义的值
            }
            else if (ast is AstFunction)
            {
                result = CalcFunction(new FuncEnvironment(env, Utils.GetScopeString()), ast as AstFunction); // 函数调用
            }
            else
            {
                throw new RunTimeException("非法节点类型: " + ast.GetType().ToString(), ast);
            }
            Utils.LogDebug("[R] 正在计算[{0}]{1}的值，结果为：{2}", ast.GetType(), ast, result);
            return result;
        }

        private static EvalValue CalcProg(IEnvironment env, AstProg ast)
        {
            var ret = new EvalValue();
            foreach (var item in ast)
            {
                try
                {
                    ret = item.Eval(env);
                }
                catch (ReturnException)
                {
                    break;
                }
            }
            if (env.Get("return@Main") != null)
            {
                return env.Get("return@Main");
            }
            return ret;
        }

        private static EvalValue CalcExpression(IEnvironment env, AstExpression ast)
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

        private static EvalValue CalcIf(IEnvironment env, AstIf ast)
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

        private static EvalValue CalcWhile(IEnvironment env, AstWhile ast)
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

        private static EvalValue CalcReturn(IEnvironment env, AstReturn ast)
        {
            var ret = ast.result.Eval(env);
            env.Set("return@" + env.GetScopeName(), ret);
            // 中断执行
            throw new ReturnException(ast);
        }

        private static EvalValue CalcString(IEnvironment env, AstStringLiteral ast)
        {
            return new EvalValue(ast.str);
        }

        private static EvalValue CalcNumber(IEnvironment env, AstNumberLiteral ast)
        {
            return new EvalValue(ast.value);
        }

        private static EvalValue CalcIdentifier(IEnvironment env, AstIdentifier ast)
        {
            var val = env.Get(ast.name);
            if (val == null)
            {
                throw new RunTimeException("未定义的标识符", ast);
            }
            return val;
        }

        private static EvalValue CalcFunction(FuncEnvironment env, AstFunction ast)
        {
            // 初始化实参
            var callee = env.GetMainEnv().GetFuncDef(ast.token.GetText());
            if (callee != null)
            {
                // 脚本方法
                var paramList = callee.paramList;
                for (var i = 0; i < paramList.length; ++i)
                {
                    var item = paramList.GetName(i);
                    if (ast.args.ChildrenCount() <= i)
                    {
                        throw new RunTimeException("实参不足", ast);
                    }
                    var argItem = ast.args.ChildAt(i) as AstExpression;
                    env.AddArg(item, argItem.Eval(env.parent));
                }
                // 执行函数体
                var funcBody = callee.block;
                var ret = new EvalValue();
                foreach (var item in funcBody)
                {
                    try
                    {
                        ret = item.Eval(env);
                    }
                    catch (ReturnException)
                    {
                        break;
                    }
                }
                if (env.Get("return@" + env.GetScopeName()) != null)
                {
                    return env.Get("return@" + env.GetScopeName());
                }
                return ret;
            }
            else
            {
                var csm = env.GetMainEnv().GetCsFuncDef(ast.token.GetText());
                if (csm != null)
                {
                    // CS方法
                    var csRet = csm(ast.args.ToArgArray(env));
                    var ret = new EvalValue(csRet);
                    return ret;
                }
            }
            throw new RunTimeException("函数未定义:" + ast.token.GetText() + "()", ast);
        }

        public static EvalValue CallFunc(FuncEnvironment env, AstFuncDef callee, params object[] args)
        {
            var paramList = callee.paramList;
            if (args.Length < paramList.length)
            {
                throw new RunTimeException(string.Format("实参不足: 需要{0}个，提供了{0}个", paramList.length, args.Length));
            }
            for (var i = 0; i < paramList.length; ++i)
            {
                var item = paramList.GetName(i);
                var argItem = args[i];
                if (argItem is System.Int32)
                {
                    env.AddArg(item, new EvalValue((int)argItem));
                }
                else if (argItem is System.String)
                {
                    env.AddArg(item, new EvalValue(argItem as System.String));
                }
                else
                {
                    throw new RunTimeException("参数类型不正确:" + argItem.GetType().ToString());
                }
            }
            // 执行函数体
            var funcBody = callee.block;
            var ret = new EvalValue();
            foreach (var item in funcBody)
            {
                try
                {
                    ret = item.Eval(env);
                }
                catch (ReturnException)
                {
                    break;
                }
            }
            if (env.Get("return@" + env.GetScopeName()) != null)
            {
                return env.Get("return@" + env.GetScopeName());
            }
            return ret;
        }
    }
}
