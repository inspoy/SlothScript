using SlothScript.AST;

namespace SlothScript
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    public class Parser
    {
        private Scanner m_scanner;

        public Parser(Scanner scanner)
        {
            m_scanner = scanner;
        }

        public AstProg DoParse()
        {
            return null;
        }
    }
}