using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 所有含有树枝的节点的父类
    /// </summary>
    internal abstract class AstList : AstNode
    {
        protected List<AstNode> m_children;
        public AstList(List<AstNode> list)
        {
            m_children = list ?? throw new RunTimeException("AstList构造参数为空");
        }

        public override AstNode ChildAt(int idx)
        {
            return m_children[idx];
        }

        public override int ChildrenCount()
        {
            return m_children.Count;
        }

        public override IEnumerator<AstNode> GetEnumerator()
        {
            return m_children.GetEnumerator();
        }

        public override string GetLocation()
        {
            foreach (AstNode node in m_children)
            {
                string s = node.GetLocation();
                if (s != null)
                {
                    return s;
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');
            string sep = "";
            foreach (var node in m_children)
            {
                sb.Append(sep);
                sep = " ";
                sb.Append(node.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}
