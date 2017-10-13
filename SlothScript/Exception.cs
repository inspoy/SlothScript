using System;
using System.Collections.Generic;
using System.Text;
using SlothScript.AST;

namespace SlothScript
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RunTimeException : Exception
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
    public class ParseException : Exception
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
}
