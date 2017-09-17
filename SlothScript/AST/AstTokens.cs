using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 整形字面量
    /// </summary>
    public class AstNumberLiteral : AstLeaf
    {
        public AstNumberLiteral(Token t) : base(t)
        { }
        public int value { get => token.GetNumber(); }
    }

    /// <summary>
    /// 变量名
    /// </summary>
    public class AstName : AstLeaf
    {
        public AstName(Token t) : base(t)
        { }
        public string name { get => token.GetText(); }
    }
}
