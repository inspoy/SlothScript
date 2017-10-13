using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 所有叶子节点（标识符，字面量）的父类
    /// </summary>
    public class AstLeaf : AstNode
    {
        private static List<AstNode> EMPTY = new List<AstNode>();
        public Token token { get => m_token; }
        protected Token m_token;
        public AstLeaf(Token t)
        {
            m_token = t;
        }

        public override AstNode ChildAt(int idx)
        {
            throw new IndexOutOfRangeException();
        }

        public override int ChildrenCount()
        {
            return 0;
        }

        public override IEnumerator<AstNode> GetEnumerator()
        {
            return EMPTY.GetEnumerator();
        }

        public override string GetLocation()
        {
            return "at line " + m_token.lineNumber.ToString();
        }

        public override string ToString()
        {
            return m_token.GetText();
        }
    }
}
