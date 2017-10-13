using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 运算表达式，包含赋值操作
    /// </summary>
    public class AstExpression : AstList
    {
        public AstExpression(List<AstNode> list) : base(list)
        { }

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
    }
}
