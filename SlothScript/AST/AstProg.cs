using System.Collections.Generic;

namespace SlothScript.AST
{
    /// <summary>
    /// 脚本程序
    /// </summary>
    public class AstProg : AstList
    {
        public AstProg(List<AstNode> list) : base(list)
        {}
    }
}