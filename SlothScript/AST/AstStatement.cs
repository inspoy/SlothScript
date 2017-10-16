using System.Collections.Generic;

namespace SlothScript.AST
{
    /// <summary>
    /// 语句,包括if,while,return
    /// </summary>
    internal abstract class AstStatement : AstList
    {
        public AstStatement(List<AstNode> list) : base(list)
        { }
    }

    /// <summary>
    /// while语句
    /// </summary>
    internal sealed class AstWhile : AstStatement
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

        public AstExpression condition { get => m_condition; }
    }

    /// <summary>
    /// if语句
    /// </summary>
    internal sealed class AstIf : AstStatement
    {
        private AstExpression m_condition;
        AstElse m_elseBlock;

        public AstIf(List<AstNode> list, AstExpression condition, List<AstNode> elseList) : base(list)
        {
            m_condition = condition;
            m_elseBlock = null;
            if (elseList != null)
            {
                m_elseBlock = new AstElse(elseList);
            }
        }

        public override string GetLocation()
        {
            return m_condition.GetLocation();
        }

        public override string ToString()
        {
            if (m_elseBlock == null)
            {
                return string.Format("(<if>{0}=>{1})", m_condition, base.ToString());
            }
            else
            {
                return string.Format("(<if>{0}=>{1}!{2})", m_condition, base.ToString(), m_elseBlock);
            }
        }

        public AstExpression condition { get => m_condition; }

        public AstElse elseBlock { get => m_elseBlock; }
    }

    /// <summary>
    /// if语句的else块
    /// </summary>
    internal sealed class AstElse : AstList
    {
        public AstElse(List<AstNode> list) : base(list)
        { }
    }

    /// <summary>
    /// return语句
    /// </summary>
    internal sealed class AstReturn : AstStatement
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

        public AstExpression result { get => m_result; }
    }
}