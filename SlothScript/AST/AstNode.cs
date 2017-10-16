using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 抽象语法树的基类
    /// </summary>
    internal abstract class AstNode : IEnumerable<AstNode>
    {
        public abstract AstNode ChildAt(int idx);
        public abstract int ChildrenCount();
        public abstract IEnumerator<AstNode> GetEnumerator();
        public abstract string GetLocation();

        /// <summary>
        /// 求代码块的值
        /// </summary>
        /// <returns></returns>
        public EvalValue Eval(IEnvironment env)
        {
            return Evaluator.Eval(env, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
