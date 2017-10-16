using System;
using System.Collections.Generic;
using System.Text;
using SlothScript.AST;

namespace SlothScript
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    internal class RunTimeException : Exception
    {
        public RunTimeException() : base()
        { }

        public RunTimeException(string msg) : base(msg)
        { }

        public RunTimeException(Exception inner) : base("", inner)
        { }

        public RunTimeException(string msg, Exception inner) : base(msg, inner)
        { }

        public RunTimeException(string msg, AstNode t) : base(msg + " " + t.GetLocation())
        { }
    }

    /// <summary>
    /// 解析时错误
    /// </summary>
    internal class ParseException : Exception
    {
        public ParseException(Token t, string msg = "") :
            base("Syntax error around " + GetLocation(t) + ". " + msg)
        { }

        public ParseException(string msg) : base(msg)
        { }

        public static string GetLocation(Token t)
        {
            if (t == Token.EOF)
            {
                return "the last line";
            }
            else
            {
                return "\"" + t.GetText() + "\" at line " + t.lineNumber.ToString();
            }
        }
    }

    /// <summary>
    /// 用于实现Break语句所使用的异常，仅在内部产生并捕获
    /// </summary>
    internal class BreakException : Exception
    {
        public readonly AstNode ast;
        public BreakException(AstNode t)
        {
            ast = t;
        }
    }

    /// <summary>
    /// 用于实现Return语句所使用的异常，仅在内部产生并捕获
    /// </summary>
    internal class ReturnException : Exception
    {
        public readonly AstNode ast;
        public ReturnException(AstNode t)
        {
            ast = t;
        }
    }
}
