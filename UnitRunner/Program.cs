using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SlothScript;

namespace UnitRunner
{
    /// <summary>
    /// 测试器
    /// </summary>
    class Runner
    {
        private static string TEST_SCRIPT = @"
sum = 0;
i = 0;
while i < 10 do
  if i % 2 == 0 do
    sum = sum + i; // 计算2+4+6...
  end;
  i = i + 1; // 循环计数器
end;
return sum; // 返回结果
";

        /// <summary>
        /// 测试词法分析器
        /// </summary>
        public void RunScanner()
        {
            var scanner = new Scanner(TEST_SCRIPT);
            Token t;
            StringBuilder res = new StringBuilder();
            do
            {
                t = scanner.Read();
                string typeString = t.GetType().ToString();
                res.Append("<" + typeString.Substring(12, typeString.Length - 17) + ":" + t.GetText() + ">");
            } while (t != Token.EOF);
            Console.WriteLine(res);
        }

        /// <summary>
        /// 测试语法分析器
        /// </summary>
        public void RunParser()
        {
            var scanner = new Scanner(TEST_SCRIPT);
            var parser = new Parser(scanner);
            var prog = parser.DoParse();
            Console.WriteLine(prog.ToString());
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    class Program
    {
        /// <summary>
        /// 单元测试程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                var runner = new Runner();
                runner.RunScanner();
            }
            catch (RunTimeException e)
            {
                Console.WriteLine("解释器运行时错误: " + e.Message);
            }
            catch (ParseException e)
            {
                Console.WriteLine("词法解析时出错: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("其他错误: " + e.Message);
            }

            Console.ReadKey();
        }
    }
}
