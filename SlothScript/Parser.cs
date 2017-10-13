using SlothScript.AST;
using System.Collections.Generic;

namespace SlothScript
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    public class Parser
    {
        private Scanner m_scanner;

        public Parser(Scanner scanner)
        {
            m_scanner = scanner;
        }

        /// <summary>
        /// 执行解析
        /// </summary>
        /// <returns></returns>
        public AstProg DoParse()
        {
            /**
             * 语法BNF
             * factor   ::==    <id>|<num>|<string>|'('<expr>')'
             * expr     ::==    <factor>{<pun><factor>}';'
             * state    ::==    <while><expr><do>{<expr>|<state>}<end>';'
             *          |       <if><expr><do>{<expr>|<state>}<end>';'
             *          |       <return><expr>';'
             * prog     ::==    {<exp>|<state>}
             */
            return new AstProg(DoBlock());
        }

        /// <summary>
        /// 解析运算子factor，返回AstFactor或AstExpression
        /// </summary>
        /// <returns></returns>
        private AstNode DoFactor()
        {
            // factor   ::==    <id>|<num>|<string>|'('<expr>')'
            if (Is("(", TokenType.SEPERATOR))
            {
                // 左括号
                Pass("(", TokenType.SEPERATOR);
                AstNode ast = DoExpression();
                Pass(")", TokenType.SEPERATOR);
                return ast;
            }
            else
            {
                Token t = m_scanner.Read();
                if (t.tokenType == TokenType.IDENTIFIER)
                {
                    // 标识符
                    return new AstIdentifier(t);
                }
                else if (t.tokenType == TokenType.NUMBER)
                {
                    // 整数
                    return new AstNumberLiteral(t);
                }
                else if (t.tokenType == TokenType.STRING)
                {
                    // 字符串
                    return new AstString(t);
                }
                else
                {
                    // 非法
                    throw new ParseException(t, "非法的Token");
                }
            }
        }

        /// <summary>
        /// 解析表达式expr
        /// </summary>
        /// <returns></returns>
        private AstExpression DoExpression()
        {
            // expr     ::==    <factor>{<pun><factor>}';'
            // pun:     == <= >= && || + - * / < > = %
            List<AstNode> list = new List<AstNode>();
            AstNode first = DoFactor();
            list.Add(first);
            while (Is(null, TokenType.PUNCT))
            {
                AstPun pun = new AstPun(m_scanner.Read());
                list.Add(pun);
                AstNode next = DoFactor();
                list.Add(next);
            }
            if (Is(";", TokenType.SEPERATOR))
            {
                // 下一个Token是分号
                Pass(";", TokenType.SEPERATOR);
            }
            else if (Is("do", TokenType.KEYWORD))
            {
                // 下一个Token是关键词'do'
            }
            else
            {
                // 都不是
                throw new ParseException(m_scanner.Peek(0), "Missing Semicolon");
            }
            var ret = new AstExpression(list);
            Utils.LogInfo("表达式: {0}", ret.ToString());
            return ret;
        }

        /// <summary>
        /// 解析语句state
        /// </summary>
        /// <returns></returns>
        private AstStatement DoStatement()
        {
            // state    ::==    <while><expr><do>{<expr>|<state>}<end>';'
            //          |       <if><expr><do>{<expr>|<state>}<end>';'
            //          |       <return><expr>';'
            if (Is("while", TokenType.KEYWORD))
            {
                // while语句
                Pass("while", TokenType.KEYWORD);
                AstExpression condition = DoExpression();
                Pass("do", TokenType.KEYWORD);
                List<AstNode> list = DoBlock();
                Pass("end", TokenType.KEYWORD);
                Pass(";", TokenType.SEPERATOR);
                var ret = new AstWhile(list, condition);
                Utils.LogInfo("while语句: {0}", ret.ToString());
                return ret;
            }
            if (Is("if", TokenType.KEYWORD))
            {
                // if语句
                Pass("if", TokenType.KEYWORD);
                AstExpression condition = DoExpression();
                Pass("do", TokenType.KEYWORD);
                List<AstNode> list = DoBlock();
                Pass("end", TokenType.KEYWORD);
                Pass(";", TokenType.SEPERATOR);
                var ret = new AstIf(list, condition);
                Utils.LogInfo("if语句: {0}", ret.ToString());
                return ret;
            }
            if (Is("return", TokenType.KEYWORD))
            {
                // return语句
                Pass("return", TokenType.KEYWORD);
                AstExpression expr = DoExpression();
                var ret = new AstReturn(new List<AstNode>(), expr);
                Utils.LogInfo("return语句: {0}", ret.ToString());
                return ret;
            }
            throw new ParseException(m_scanner.Peek(0), "Unknown statement");
        }

        /// <summary>
        /// 解析语句块
        /// </summary>
        /// <returns></returns>
        private List<AstNode> DoBlock()
        {
            // {<exp>|<state>}
            List<AstNode> ret = new List<AstNode>();
            while (true)
            {
                if (Is("EOF", TokenType.INVALID))
                {
                    // 文件结束
                    break;
                }
                if (Is("end", TokenType.KEYWORD))
                {
                    // 遇到了end关键字
                    break;
                }
                if (Is("while", TokenType.KEYWORD) || Is("if", TokenType.KEYWORD) || Is("return", TokenType.KEYWORD))
                {
                    // 语句
                    ret.Add(DoStatement());
                }
                else
                {
                    // 表达式
                    ret.Add(DoExpression());
                }
            }
            return ret;
        }

        /// <summary>
        /// 下一个Token的name和type是否为指定值
        /// </summary>
        /// <param name="name">为null时表示任意</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool Is(string name, TokenType type)
        {
            Token t = m_scanner.Peek(0);
            bool checkName = true;
            if (name != null)
            {
                checkName = t.GetText().Equals(name);
            }
            bool checkType = t.tokenType == type;
            return checkName && checkType;
        }

        /// <summary>
        /// 扫描位置跳过指定name和type的Token，如果name不符合则抛出异常
        /// </summary>
        /// <param name="name">为null时表示任意</param>
        /// <param name="type"></param>
        private void Pass(string name, TokenType type)
        {
            Token t = m_scanner.Read();
            bool checkName = true;
            if (name != null)
            {
                checkName = t.GetText().Equals(name);
            }
            bool checkType = t.tokenType == type;
            if (!checkName || !checkType)
            {
                throw new ParseException(t, "非法的Token: " + name);
            }
        }
    }
}