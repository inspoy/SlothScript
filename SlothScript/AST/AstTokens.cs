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

        public static int GetPriority(AstPun val)
        {
            string op = val.token.ToString();
            int ret = 0;
            if (op == "*" || op == "/" || op == "%")
            {
                ret = 3;
            }
            else if (op == "+" || op == "-")
            {
                ret = 2;
            }
            else if (op == ">=" || op == "<=" || op == "==" || op == ">" || op == "<")
            {
                ret = 1;
            }
            else if (op == "&&" || op == "||")
            {
                ret = 0;
            }
            else
            {
                throw new RunTimeException("非法的运算符", val);
            }
            return ret;
        }

        public static bool operator >=(AstPun lhs, AstPun rhs)
        {
            return GetPriority(lhs) >= GetPriority(rhs);
        }

        public static bool operator <=(AstPun lhs, AstPun rhs)
        {
            return !(lhs >= rhs) || lhs.token.ToString().Equals(rhs.token.ToString());
        }
    }
}
