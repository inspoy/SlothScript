using System;
using System.Collections.Generic;
using System.Text;
using SlothScript.AST;

namespace SlothScript
{
    public enum EvalValueType
    {
        INT = 0,
        STRING = 1,
    }

    public class EvalValue
    {
        public EvalValue()
        { }

        public EvalValue(int val)
        {
            intVal = val;
            type = EvalValueType.INT;
        }

        public EvalValue(string val)
        {
            strVal = val;
            type = EvalValueType.STRING;
        }

        public EvalValueType type = EvalValueType.INT;
        public int intVal = 0;
        public string strVal = "";
        public bool hasReturned = false;
    }

    public static class Evaluator
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
                result = CalcProg(env,ast as AstProg);
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
            Utils.LogInfo("[R] 正在计算[{0}]{1}的值，结果为：{2}", ast.GetType().ToString(), ast.ToString(), result);
            return result;
        }

        private static EvalValue CalcProg(Environment env, AstProg ast)
        {
            var ret = new EvalValue();
            foreach (var item in ast)
            {
                ret = item.Eval(env);
            }
            return ret; // TODO: 应当返回虚拟机中保存的return值
        }

        private static EvalValue CalcExpression(Environment env, AstExpression ast)
        {
            // TODO
            return new EvalValue();
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
            // TODO: 将ret放到虚拟机中，并中断执行
            ret.hasReturned = true;
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
            // TODO
            return new EvalValue();
        }
    }
}
