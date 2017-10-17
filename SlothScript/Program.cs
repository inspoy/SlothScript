using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlothScript
{
    /// <summary>
    /// 编译结果
    /// </summary>
    public class CompileResult
    {
        /// <summary>
        /// 编译是否成功
        /// </summary>
        public bool success = false;

        /// <summary>
        /// 编译信息
        /// </summary>
        public string output = "";

        public override string ToString()
        {
            string ret;
            if (success)
            {
                ret = "编译成功";
            }
            else
            {
                ret = "编译失败，错误信息：" + output;
            }
            return ret;
        }
    }

    /// <summary>
    /// 运行结果
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// 运行是否成功
        /// </summary>
        public bool success = false;

        /// <summary>
        /// 返回值
        /// </summary>
        public string returnVal = "";

        /// <summary>
        /// 运行失败时的错误信息
        /// </summary>
        public string error = "";

        public override string ToString()
        {
            string ret;
            if (success)
            {
                ret = "运行成功，返回值=" + returnVal;
            }
            else
            {
                ret = "运行失败，错误信息：" + error;
            }
            return ret;
        }
    }

    /// <summary>
    /// 脚本程序
    /// </summary>
    public class Program
    {
        private bool m_compiled = false;
        /// <summary>
        /// 是否已经编译成功
        /// </summary>
        public bool isCompiled { get => m_compiled; }

        private string m_code;
        private Scanner m_scanner;
        private Parser m_parser;
        private AST.AstProg m_prog;

        internal MainEnvironment mainEnv;

        /// <summary>
        /// 测试用，词法分析器
        /// </summary>
        /// <param name="code"></param>
        public static void TestScanner(string code)
        {
            var scanner = new Scanner(code);
            Token t;
            StringBuilder res = new StringBuilder();
            do
            {
                t = scanner.Read();
                string typeString = t.GetType().ToString();
                res.Append("<" + typeString.Substring(12, typeString.Length - 17) + ":" + t.GetText() + ">");
            } while (t != Token.EOF);
            Console.WriteLine("[TEST] - " + res);
        }

        /// <summary>
        /// 测试用，语法分析器
        /// </summary>
        /// <param name="code"></param>
        public static void TestParser(string code)
        {
            var scanner = new Scanner(code);
            var parser = new Parser(scanner);
            Console.WriteLine("[TEST] - " + parser.DoParse().ToString());
        }

        public Program(string code)
        {
            m_code = code;
        }

        /// <summary>
        /// 编译脚本程序
        /// </summary>
        /// <returns></returns>
        public CompileResult Compile()
        {
            var result = new CompileResult();
            if (m_compiled)
            {
                result.success = true;
                result.output = "重复编译，已经跳过";
            }
            try
            {
                m_scanner = new Scanner(m_code);
                m_parser = new Parser(m_scanner);
                m_prog = m_parser.DoParse();
                result.success = true;
                m_compiled = true;
            }
            catch (ParseException e)
            {
                result.output = "编译错误：" + e.Message;
            }
            catch (Exception e)
            {
                result.output = string.Format("其他错误[{0}]-{1}", e.GetType().ToString(), e.Message);
            }
            Utils.LogDebug("脚本编译完毕：" + result.ToString());
            return result;
        }

        /// <summary>
        /// 运行脚本程序
        /// </summary>
        /// <returns></returns>
        public RunResult Run(VM vm = null)
        {
            var ret = new RunResult();
            if (!isCompiled)
            {
                ret.error = "尚未编译";
                return ret;
            }
            mainEnv = new MainEnvironment(vm);
            try
            {
                ret.returnVal = m_prog.Eval(mainEnv).ToString();
                ret.success = true;
            }
            catch (RunTimeException e)
            {
                mainEnv = null;
                ret.error = "运行时错误：" + e.Message;
            }
            catch (Exception e)
            {
                mainEnv = null;
                ret.error = string.Format("其他错误[{0}]-{1}", e.GetType().ToString(), e.Message);
            }
            Utils.LogDebug("脚本执行完毕：" + ret.ToString());
            return ret;
        }
    }
}
