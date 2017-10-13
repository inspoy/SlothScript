using System.Collections.Generic;

namespace SlothScript.AST
{
    /// <summary>
    /// 语句,包括if,while,return
    /// </summary>
    public class AstStatement : AstList
    {
        public AstStatement(List<AstNode> list) : base(list)
        {
        }
    }

    /// <summary>
    /// while语句
    /// </summary>
    public class AstWhile : AstStatement
    {
        private AstExpression m_condition;

        public AstWhile(List<AstNode> list, AstExpression condition) : base(list)
        {
            m_condition = condition;
        }
    }

    /// <summary>
    /// if语句
    /// </summary>
    public class AstIf : AstStatement
    {
        private AstExpression m_condition;

        public AstIf(List<AstNode> list, AstExpression condition) : base(list)
        {
            m_condition = condition;
        }
    }

    /// <summary>
    /// return语句
    /// </summary>
    public class AstReturn : AstStatement
    {
        private AstExpression m_result;

        public AstReturn(List<AstNode> list, AstExpression result) : base(list)
        {
            m_result = result;
        }
    }
}