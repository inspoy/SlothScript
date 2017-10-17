using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlothScript.AST
{
    /// <summary>
    /// 函数语法树节点
    /// </summary>
    internal class AstFuncDef : AstList
    {
        /// <summary>
        /// list[0]:[AstIdentifier]函数名
        /// list[1]:[AstParameters]参数列表
        /// list[2]:[AstFuncBody]函数体
        /// </summary>
        /// <param name="list"></param>
        public AstFuncDef(List<AstNode> list) : base(list)
        {
            if (list.Count() != 3)
            {
                throw new ParseException("AstFuncDef的构造函数只能接受三个元素的List");
            }
        }

        /// <summary>
        /// 函数名
        /// </summary>
        public string name { get => (ChildAt(0) as AstIdentifier).token.GetText(); }

        /// <summary>
        /// 参数列表
        /// </summary>
        public AstParameters paramList { get => ChildAt(1) as AstParameters; }

        /// <summary>
        /// 函数体
        /// </summary>
        public AstFuncBody block { get => ChildAt(2) as AstFuncBody; }
    }

    /// <summary>
    /// 函数体本身
    /// </summary>
    internal class AstFuncBody : AstList
    {
        public AstFuncBody(List<AstNode> list) : base(list)
        {
        }
    }

    /// <summary>
    /// 函数参数列表
    /// </summary>
    internal class AstParameters : AstList
    {
        public AstParameters(List<AstNode> list) : base(list)
        { }

        public string GetName(int i)
        {
            return (ChildAt(i) as AstIdentifier).token.GetText();
        }

        public int length { get => ChildrenCount(); }
    }

    /// <summary>
    /// 调用函数的表达式
    /// </summary>
    internal class AstFunction : AstFactor
    {
        public readonly AstArgs args;
        public AstFunction(Token t, AstArgs _args) : base(t)
        {
            args = _args;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}", token, args);
        }
    }

    /// <summary>
    /// 函数实参列表
    /// </summary>
    internal class AstArgs : AstList
    {
        public AstArgs(List<AstNode> list) : base(list)
        {
        }

        public object[] ToArgArray(IEnvironment env)
        {
            object[] ret = new object[ChildrenCount()];
            for (int i = 0; i < ChildrenCount(); ++i)
            {
                var expr = ChildAt(i);
                var val = expr.Eval(env);
                if (val.type == EvalValueType.INT)
                {
                    ret[i] = val.intVal;
                }
                else if (val.type == EvalValueType.STRING)
                {
                    ret[i] = val.strVal;
                }
            }
            return ret;
        }
    }
}
