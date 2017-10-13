using System.Collections.Generic;

namespace SlothScript.AST
{
    /// <summary>
    /// 语句,包括if,while,return
    /// </summary>
    public class AstStatement : AstList
    {
        public AstStatement(List<AstNode> list) : base(list)
        { }
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

        public override string GetLocation()
        {
            return m_condition.GetLocation();
        }

        public override string ToString()
        {
            return string.Format("(<while>{0}=>{1})", m_condition.ToString(), base.ToString());
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

        public override string GetLocation()
        {
            return m_condition.GetLocation();
        }

        public override string ToString()
        {
            return string.Format("(<if>{0}=>{1})", m_condition.ToString(), base.ToString());
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

        public override string GetLocation()
        {
            return m_result.GetLocation();
        }

        public override string ToString()
        {
            return string.Format("(<return>{0})", m_result.ToString());
        }
    }
}