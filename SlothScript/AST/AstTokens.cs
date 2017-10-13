using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 运算子
    /// </summary>
    public class AstFactor : AstLeaf
    {
        public AstFactor(Token t) : base(t)
        { }
    }

    /// <summary>
    /// 整形字面量
    /// </summary>
    public class AstNumberLiteral : AstFactor
    {
        public AstNumberLiteral(Token t) : base(t)
        { }
        public int value { get => token.GetNumber(); }
    }

    /// <summary>
    /// 变量名
    /// </summary>
    public class AstName : AstFactor
    {
        public AstName(Token t) : base(t)
        { }
        public string name { get => token.GetText(); }
    }

    /// <summary>
    /// 字符串字面量
    /// </summary>
    public class AstString : AstFactor
    {
        public AstString(Token t) : base(t)
        { }
        public string str { get => token.GetText(); }
    }

    public class AstPun : AstLeaf
    {
        public AstPun(Token t) : base(t)
        { }
    }
}
