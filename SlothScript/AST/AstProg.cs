using System.Collections.Generic;

namespace SlothScript.AST
{
    /// <summary>
    /// 脚本程序
    /// </summary>
    public sealed class AstProg : AstList
    {
        public AstProg(List<AstNode> list) : base(list)
        {}
    }
}