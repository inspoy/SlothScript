using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 运算子
    /// </summary>
    internal abstract class AstFactor : AstLeaf
    {
        public AstFactor(Token t) : base(t)
        { }
    }

    /// <summary>
    /// 整形字面量
    /// </summary>
    internal sealed class AstNumberLiteral : AstFactor
    {
        public AstNumberLiteral(Token t) : base(t)
        { }
        public int value { get => token.GetNumber(); }
    }

    /// <summary>
    /// 标识符
    /// </summary>
    internal sealed class AstIdentifier : AstFactor
    {
        public AstIdentifier(Token t) : base(t)
        { }
        public string name { get => token.GetText(); }
    }

    /// <summary>
    /// 字符串字面量
    /// </summary>
    internal sealed class AstStringLiteral : AstFactor
    {
        public AstStringLiteral(Token t) : base(t)
        { }
        public string str { get => token.GetText(); }
    }

    /// <summary>
    /// 运算符
    /// </summary>
    internal sealed class AstPun : AstLeaf
    {
        public AstPun(Token t) : base(t)
        { }
        public string pun { get => token.GetText(); }
    }
}
