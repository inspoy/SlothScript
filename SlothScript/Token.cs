using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript
{
    /// <summary>
    /// 单词类型
    /// </summary>
    internal enum TokenType
    {
        INVALID = 0,

        /// <summary>
        /// 标识符
        /// </summary>
        IDENTIFIER = 1,

        /// <summary>
        /// 数字（整数）
        /// </summary>
        NUMBER = 2,

        /// <summary>
        /// 字符串
        /// </summary>
        STRING = 3,

        /// <summary>
        /// 标点符号
        /// </summary>
        PUNCT = 4,

        /// <summary>
        /// 关键字
        /// </summary>
        KEYWORD = 5,

        /// <summary>
        /// 分隔符
        /// </summary>
        SEPERATOR = 6,
    }

    /// <summary>
    /// 单词基类
    /// 这是一个抽象类，不可直接使用
    /// </summary>
    internal abstract class Token
    {
        public static Token EOF = new EofToken(-1);

        private int m_lineNumber;
        public int lineNumber { get => m_lineNumber; }

        protected TokenType m_tokenType;
        public TokenType tokenType { get => m_tokenType; }

        protected Token(int line)
        {
            m_tokenType = TokenType.INVALID;
            m_lineNumber = line;
        }

        public virtual int GetNumber()
        {
            throw new RunTimeException("This is NOT a number token");
        }

        public virtual string GetText()
        {
            return "";
        }

        public override string ToString()
        {
            return GetText();
        }
    }

    /// <summary>
    /// 标示文件结束的Token
    /// </summary>
    internal class EofToken : Token
    {
        public EofToken(int line) : base(line)
        {
            m_tokenType = TokenType.INVALID;
        }
        public override string GetText()
        {
            return "EOF";
        }
    }

    /// <summary>
    /// 标识符Token
    /// </summary>
    internal class IdToken : Token
    {
        private string m_text;
        public IdToken(int line, string value) : base(line)
        {
            m_tokenType = TokenType.IDENTIFIER;
            m_text = value;
        }
        public override string GetText()
        {
            return m_text;
        }
    }

    /// <summary>
    /// 整数Token
    /// </summary>
    internal class NumToken : Token
    {
        private int m_number;
        public NumToken(int line, int value) : base(line)
        {
            m_tokenType = TokenType.NUMBER;
            m_number = value;
        }
        public override string GetText()
        {
            return m_number.ToString();
        }
        public override int GetNumber()
        {
            return m_number;
        }
    }

    /// <summary>
    /// 字符串Token
    /// </summary>
    internal class StrToken : Token
    {
        private string m_string;
        public StrToken(int line, string value) : base(line)
        {
            m_tokenType = TokenType.STRING;
            m_string = value;
        }
        public override string GetText()
        {
            return m_string;
        }
    }

    /// <summary>
    /// 运算符Token
    /// </summary>
    internal class PunToken : Token
    {
        private string m_punct;
        public PunToken(int line, string value) : base(line)
        {
            m_tokenType = TokenType.PUNCT;
            m_punct = value;
        }
        public override string GetText()
        {
            return m_punct;
        }
    }

    /// <summary>
    /// 关键字Token
    /// </summary>
    internal class KeyToken : Token
    {
        private string m_string;
        public KeyToken(int line, string value) : base(line)
        {
            m_tokenType = TokenType.KEYWORD;
            m_string = value;
        }
        public override string GetText()
        {
            return m_string;
        }
    }

    /// <summary>
    /// 分隔符Token，包括分号';'，括号'(',')'，逗号','
    /// </summary>
    internal class SepToken : Token
    {
        private string m_string;
        public SepToken(int line, string value) : base(line)
        {
            m_tokenType = TokenType.SEPERATOR;
            m_string = value;
        }
        public override string GetText()
        {
            return m_string;
        }
    }
}
