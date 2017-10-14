﻿using SlothScript.AST;
using System.Collections.Generic;
using System;

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
        private AstProg DoParse()
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
            // factor   ::==    <id>|<num>|<string>|'('<expr>')
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
                    return new AstStringLiteral(t);
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
            // pun:     != == <= >= && || + - * / < > = %
            List<AstNode> list = new List<AstNode>();
            AstExpression ret;
            AstNode first = DoFactor();
            if (Is("=", TokenType.PUNCT))
            {
                // 赋值操作
                if (!(first is AstIdentifier))
                {
                    throw new ParseException(m_scanner.Peek(0), "非法的左值");
                }
                ret = new AstExpression(list, (first as AstIdentifier).token.ToString());
                Pass("=", TokenType.PUNCT);
                first = DoFactor();
            }
            else
            {
                ret = new AstExpression(list);
            }
            list.Add(first);
            while (Is(null, TokenType.PUNCT))
            {
                AstPun pun = new AstPun(m_scanner.Read());
                if (pun.token.ToString() == "=")
                {
                    throw new ParseException(pun.token, "非法的赋值运算符");
                }
                list.Add(pun);
                AstNode next = DoFactor();
                list.Add(next);
            }
            if (Is(";", TokenType.SEPERATOR))
            {
                // 下一个Token是分号
                Pass(";", TokenType.SEPERATOR);
            }
            else if (Is("do", TokenType.KEYWORD) || Is(")", TokenType.SEPERATOR))
            {
                // 下一个Token是关键词'do'或者右括号')'
            }
            else
            {
                // 都不是
                throw new ParseException(m_scanner.Peek(0), "缺少分号");
            }
            Utils.LogDebug("[P] 表达式: {0}", ret.ToString());
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
                Utils.LogDebug("[P] while语句: {0}", ret.ToString());
                return ret;
            }
            if (Is("if", TokenType.KEYWORD))
            {
                // if语句
                Pass("if", TokenType.KEYWORD);
                AstExpression condition = DoExpression();
                Pass("do", TokenType.KEYWORD);
                List<AstNode> list = DoBlock();
                List<AstNode> elseList = null;
                if (Is("else", TokenType.KEYWORD))
                {
                    // 有else块
                    elseList = DoBlock();
                }
                Pass("end", TokenType.KEYWORD);
                Pass(";", TokenType.SEPERATOR);
                var ret = new AstIf(list, condition, elseList);
                Utils.LogDebug("[P] if语句: {0}", ret.ToString());
                return ret;
            }
            if (Is("return", TokenType.KEYWORD))
            {
                // return语句
                Pass("return", TokenType.KEYWORD);
                AstExpression expr = DoExpression();
                var ret = new AstReturn(new List<AstNode>(), expr);
                Utils.LogDebug("[P] return语句: {0}", ret.ToString());
                return ret;
            }
            throw new ParseException(m_scanner.Peek(0), "未知语句");
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
                if (Is("end", TokenType.KEYWORD) || Is("else", TokenType.KEYWORD))
                {
                    // 遇到了end或else关键字
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

        /// <summary>
        /// [测试用]生成语法树生成的字符串
        /// </summary>
        /// <returns></returns>
        public string GetDumpString()
        {
            return DoParse().ToString();
        }

        /// <summary>
        /// [测试用]获取程序返回值
        /// </summary>
        /// <returns></returns>
        public string GetResult()
        {
            return DoParse().Eval(new Environment()).ToString();
        }
    }
}