using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 双目运算表达式
    /// </summary>
    public class AstBinaryExpr : AstList
    {
        public AstBinaryExpr(List<AstNode> list) : base(list)
        { }

        /// <summary>
        /// 左操作数
        /// </summary>
        public AstNode left { get => ChildAt(0); }

        /// <summary>
        /// 运算符
        /// </summary>
        public string op { get => ChildAt(1).ToString(); }

        /// <summary>
        /// 右操作数
        /// </summary>
        public AstNode right { get => ChildAt(2); }
    }
}
