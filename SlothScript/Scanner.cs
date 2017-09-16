﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SlothScript
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    public class Scanner
    {
        public static string REGEX_COMMET = @"(//.*)";
        public static string REGEX_ID = @"([A-Z_a-z]\w*)";
        public static string REGEX_NUMBER = @"(\d+)";
        public static string REGEX_STRING = @"(""(\\*|\\\\|\\n|[^""])*"")";
        public static string REGEX_PUNCT = @"(==|<=|>=|&&|\|\||[+\-\*/<>=\.;])";
        public static string REGEX_TOTAL = @"\s*(" + REGEX_COMMET + "|" + REGEX_ID + "|" + REGEX_NUMBER + "|" + REGEX_STRING + "|" + REGEX_PUNCT + ")?";

        private Regex m_pattern;
        private Regex m_patternComment;
        private Regex m_patternId;
        private Regex m_patternNumber;
        private Regex m_patternString;
        private Regex m_patternPunct;
        private List<Token> m_tokenQueue;
        private bool m_hasMore;
        private List<string> m_fileData;
        private int m_readingPos;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="src">脚本源代码</param>
        public Scanner(string src)
        {
            m_pattern = new Regex(REGEX_TOTAL);
            m_patternComment = new Regex(REGEX_COMMET);
            m_patternId = new Regex(REGEX_ID);
            m_patternNumber = new Regex(REGEX_NUMBER);
            m_patternString = new Regex(REGEX_STRING);
            m_patternPunct = new Regex(REGEX_PUNCT);
            m_tokenQueue = new List<Token>();
            m_hasMore = true;
            src = src.Replace("\r\n", "\n");
            m_fileData = new List<string>(src.Split('\n'));
            m_readingPos = 0;
        }

        /// <summary>
        /// 读取一个Token并从队列中删除
        /// </summary>
        /// <returns>读取的Token</returns>
        public Token Read()
        {
            if (FillQueue(0))
            {
                var tmp = m_tokenQueue[0];
                m_tokenQueue.RemoveAt(0);
                return tmp;
            }
            else
            {
                Utils.LogInfo("文件读取完毕");
                return Token.EOF;
            }
        }

        /// <summary>
        /// 读取队列中指定的Token但不删除
        /// </summary>
        /// <param name="idx">第几个，从0开始</param>
        /// <returns>读取的Token</returns>
        public Token Peek(int idx)
        {
            if (FillQueue(idx))
            {
                return m_tokenQueue[idx];
            }
            else
            {
                return Token.EOF;
            }
        }

        /// <summary>
        /// 加载指定个数的Token到队列，如果文件已经读完了则返回false
        /// </summary>
        /// <param name="i"></param>
        /// <returns>是否填充成功</returns>
        private bool FillQueue(int i)
        {
            while (i >= m_tokenQueue.Count)
            {
                if (m_hasMore)
                {
                    ReadLine();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 读取一行源文件
        /// </summary>
        private void ReadLine()
        {
            if (m_readingPos >= m_fileData.Count)
            {
                m_hasMore = false;
                return;
            }
            string line = m_fileData[m_readingPos++];
            Utils.LogInfo("读取了第{0}行: {1}", m_readingPos, line);
            var matches = m_pattern.Matches(line);
            StringBuilder info = new StringBuilder();
            foreach (Match item in matches)
            {
                string trimed = item.ToString().Trim();
                if (trimed == "")
                {
                    // 空语句
                    continue;
                }
                AddToken(m_readingPos, trimed);
                info.Append(" [");
                info.Append(trimed);
                info.Append("]");
            }
            if (info.ToString().Length > 0)
            {
                Utils.LogInfo("匹配到了Token:" + info.ToString());
            }
        }

        /// <summary>
        /// 往队列中添加一个Token
        /// </summary>
        /// <param name="lineNumber">Token所在行号</param>
        /// <param name="item">Token源字符串</param>
        private void AddToken(int lineNumber, string item)
        {
            Token token = null;
            if (m_patternComment.IsMatch(item))
            {
                // 注释，忽略掉
            }
            else if (m_patternId.IsMatch(item))
            {
                // 标识符
                token = new IdToken(lineNumber, item);
            }
            else if (m_patternNumber.IsMatch(item))
            {
                // 整形字面量
                if (int.TryParse(item, out int val))
                {
                    token = new NumToken(lineNumber, val);
                }
                else
                {
                    throw new ParseException("解析整形字面量失败");
                }
            }
            else if (m_patternString.IsMatch(item))
            {
                // 字符串字面量
                token = new StrToken(lineNumber, item);
            }
            else if (m_patternPunct.IsMatch(item))
            {
                // 运算符
                token = new PunToken(lineNumber, item);
            }
            if (token != null)
            {
                m_tokenQueue.Add(token);
                //Utils.LogInfo("添加了Token: " + token.GetText());
            }
        }

        /// <summary>
        /// 把源文件中的字符串字面量去转义
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private string GetStringLiteral(string src)
        {
            StringBuilder sb = new StringBuilder();
            int len = src.Length - 1;
            for (int i = 1; i < len; ++i)
            {
                char c = src[i];
                if ((c == '\\') && (i + 1 < len))
                {
                    // 找到转义符并且后边跟着其他字符
                    int c2 = src[i + 1];
                    if (c2 == '"' || c2 == '\\')
                    {
                        // 双引号或反斜杠
                        c = src[++i];
                    }
                    else if (c2 == 'n')
                    {
                        ++i;
                        c = '\n';
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}