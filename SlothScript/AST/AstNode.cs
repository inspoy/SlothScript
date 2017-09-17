using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 抽象语法树的基类
    /// </summary>
    public abstract class AstNode : IEnumerable<AstNode>
    {
        public abstract AstNode ChildAt(int idx);
        public abstract int ChildrenCount();
        public abstract IEnumerator<AstNode> GetEnumerator();
        public abstract string GetLocation();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
