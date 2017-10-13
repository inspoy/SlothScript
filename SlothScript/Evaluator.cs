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
        // 下面这个字段用于标识是否应该直接中断执行
        //public bool hasReturned = false;

        public override string ToString()
        {
            if (type == EvalValueType.INT)
            {
                return intVal.ToString();
            }
            else
            {
                return strVal;
            }
        }
    }

    internal static class Evaluator
    {
        /// <summary>
        /// 对节点进行求值
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        public static EvalValue Eval(AstNode ast)
        {
            var result = new EvalValue();
            if (ast is AstProg)
            {
                result = CalcProg(ast as AstProg);
            }
            else if (ast is AstExpression)
            {
                result = CalcExpression(ast as AstExpression);
            }
            else if (ast is AstIf)
            {
                result = CalcIf(ast as AstIf);
            }
            else if (ast is AstWhile)
            {
                result = CalcWhile(ast as AstWhile);
            }
            else if (ast is AstReturn)
            {
                result = CalcReturn(ast as AstReturn);
            }
            else if (ast is AstStringLiteral)
            {
                result = CalcString(ast as AstStringLiteral);
            }
            else if (ast is AstNumberLiteral)
            {
                result = CalcNumber(ast as AstNumberLiteral);
            }
            else if (ast is AstIdentifier)
            {
                result = CalcIdentifier(ast as AstIdentifier);
            }
            else
            {
                throw new RunTimeException("非法节点类型: " + ast.GetType().ToString(), ast);
            }
            Utils.LogDebug("[R] 正在计算[{0}]{1}的值，结果为：{2}", ast.GetType(), ast, result);
            return result;
        }

        private static EvalValue CalcProg(AstProg ast)
        {
            var ret = new EvalValue();
            foreach (var item in ast)
            {
                ret = item.Eval();
            }
            return ret; // TODO: 应当返回虚拟机中保存的return值
        }

        private static EvalValue CalcExpression(AstExpression ast)
        {
            // TODO
            return new EvalValue();
        }

        private static EvalValue CalcIf(AstIf ast)
        {
            // 返回值为执行的最后一条语句或表达式的值
            var ret = new EvalValue();
            var cond = ast.condition.Eval();
            if (cond.type == EvalValueType.STRING)
            {
                throw new RunTimeException("条件表达式不能为字符串", ast.condition);
            }
            else if (cond.type == EvalValueType.INT && cond.intVal > 0)
            {
                foreach (var item in ast)
                {
                    ret = item.Eval();
                }
            }
            else if (ast.elseBlock != null)
            {
                // 有else块
                foreach (var item in ast.elseBlock)
                {
                    ret = item.Eval();
                }
            }

            return ret;
        }

        private static EvalValue CalcWhile(AstWhile ast)
        {
            // 返回值为最后一条语句或表达式的值
            var ret = new EvalValue();
            while (true)
            {
                var cond = ast.condition.Eval();
                if (cond.type == EvalValueType.INT && cond.intVal > 0)
                {
                    // 当条件满足时执行代码块中的每一条子语句
                    foreach (var item in ast)
                    {
                        ret = item.Eval();
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

        private static EvalValue CalcReturn(AstReturn ast)
        {
            var ret = ast.result.Eval();
            // TODO: 将ret放到虚拟机中，并中断执行
            //ret.hasReturned = true;
            return ret;
        }

        private static EvalValue CalcString(AstStringLiteral ast)
        {
            return new EvalValue(ast.str);
        }

        private static EvalValue CalcNumber(AstNumberLiteral ast)
        {
            return new EvalValue(ast.value);
        }

        private static EvalValue CalcIdentifier(AstIdentifier ast)
        {
            // TODO
            return new EvalValue();
        }
    }
}
