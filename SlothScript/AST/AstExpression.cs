using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 运算表达式，包含赋值操作
    /// </summary>
    internal sealed class AstExpression : AstList
    {
        public AstExpression(List<AstNode> list) : base(list)
        {
            isAssignExpression = false;
            leftFactor = "";
        }

        public AstExpression(List<AstNode> list, string left) : base(list)
        {
            isAssignExpression = true;
            leftFactor = left;
        }

        public readonly bool isAssignExpression;
        public readonly string leftFactor;

        /// <summary>
        /// 操作数,索引从0开始
        /// </summary>
        public AstFactor GetFactor(int idx)
        {
            if (idx * 2 > ChildrenCount())
            {
                return null;
            }
            var ret = ChildAt(idx * 2);
            if (ret is AstFactor)
            {
                return ret as AstFactor;
            }
            else
            {
                throw new ParseException("不是有效的运算子");
            }
        }

        /// <summary>
        /// 运算符,索引从1开始
        /// </summary>
        public AstPun GetOperator(int idx)
        {
            if (idx * 2 > ChildrenCount())
            {
                return null;
            }
            var ret = ChildAt(idx * 2 - 1);
            if (ret is AstPun)
            {
                return ret as AstPun;
            }
            else
            {
                throw new ParseException("不是有效运算符");
            }
        }

        public override string ToString()
        {
            if (isAssignExpression)
            {
                string ret = base.ToString().Substring(1);
                ret = "(" + leftFactor + " = " + ret;
                return ret;
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
