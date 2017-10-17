using SlothScript.AST;
using System.Collections.Generic;
using System.Linq;

namespace SlothScript
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    internal class Parser
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
             * factor   ::==    <id>'('[<args>]')'|<id>|<num>|<string>|'('<expr>')'
             * expr     ::==    <factor>{<pun><factor>}';'
             * state    ::==    <while><expr><do>{<expr>|<state>}<end>';'
             *          |       <if><expr><do>{<expr>|<state>}<end>';'
             *          |       <return><expr>';'
             * func     ::==    <def><id>'('[<params>]')'<do>{<expr>|<state>}<end>';'
             * params   ::==    <id>{','<id>}
             * args     ::==    <expr>{','<expr>}
             * prog     ::==    {<exp>|<state>}
             */
            return DoProg();
        }

        private AstProg DoProg()
        {
            List<AstNode> ret = new List<AstNode>();
            while (true)
            {
                if (Is("EOF", TokenType.INVALID))
                {
                    // 文件结束
                    break;
                }

                if (Is("def", TokenType.KEYWORD))
                {
                    // 函数定义
                    ret.Add(DoFunction());
                }
                else if (Is("while", TokenType.KEYWORD) || Is("if", TokenType.KEYWORD) || Is("return", TokenType.KEYWORD))
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
            return new AstProg(ret);
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
                    if (Is("(", TokenType.SEPERATOR))
                    {
                        // 调用函数
                        Pass("(", TokenType.SEPERATOR);
                        var args = DoArgs();
                        Pass(")", TokenType.SEPERATOR);
                        return new AstFunction(t, args);
                    }
                    else
                    {
                        // 变量
                        return new AstIdentifier(t);
                    }
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
                else if (t.tokenType == TokenType.KEYWORD && t.GetText() == "def")
                {
                    // 函数定义
                    throw new ParseException(t, "函数只能在语句之前定义");
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
            else if (Is("do", TokenType.KEYWORD) || Is(")", TokenType.SEPERATOR) || Is(",", TokenType.SEPERATOR))
            {
                // 下一个Token是关键词'do'或者右括号')'或者逗号','
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
                    Pass("else", TokenType.KEYWORD);
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
        /// 解析函数定义func
        /// </summary>
        /// <returns></returns>
        private AstFuncDef DoFunction()
        {
            Pass("def", TokenType.KEYWORD);
            var list = new List<AstNode>
            {
                new AstIdentifier(Pass(null, TokenType.IDENTIFIER))
            };
            Pass("(", TokenType.SEPERATOR);
            list.Add(DoParams());
            Pass(")", TokenType.SEPERATOR);
            Pass("do", TokenType.KEYWORD);
            list.Add(new AstFuncBody(DoBlock()));
            Pass("end", TokenType.KEYWORD);
            Pass(";", TokenType.SEPERATOR);
            return new AstFuncDef(list);
        }

        /// <summary>
        /// 解析形参列表params
        /// </summary>
        /// <returns></returns>
        private AstParameters DoParams()
        {
            if (Is(")", TokenType.SEPERATOR))
            {
                // 没有参数
                return new AstParameters(new List<AstNode>());
            }
            var list = new List<AstNode>();
            var t = Pass(null, TokenType.IDENTIFIER);
            list.Add(new AstIdentifier(t));
            while (Is(",", TokenType.SEPERATOR))
            {
                Pass(",", TokenType.SEPERATOR);
                t = Pass(null, TokenType.IDENTIFIER);
                list.Add(new AstIdentifier(t));
            }
            return new AstParameters(list);
        }

        /// <summary>
        /// 解析实参列表args
        /// </summary>
        /// <returns></returns>
        private AstArgs DoArgs()
        {
            if (Is(")", TokenType.SEPERATOR))
            {
                // 没有参数
                return new AstArgs(new List<AstNode>());
            }
            var list = new List<AstNode>
            {
                DoExpression()
            };
            while (Is(",", TokenType.SEPERATOR))
            {
                Pass(",", TokenType.SEPERATOR);
                list.Add(DoExpression());
            }
            return new AstArgs(list);
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
        private Token Pass(string name, TokenType type)
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
            return t;
        }
    }
}