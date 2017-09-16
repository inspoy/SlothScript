using System;
using System.Collections.Generic;
using System.Text;
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
  sum = sum + i;
  i = i + 1; // 循环计数器
end
return sum; // 返回结果
";

        /// <summary>
        /// 测试词法分析器
        /// </summary>
        public void RunScanner()
        {
            var scanner = new Scanner(TEST_SCRIPT);
            Token t;
            string res = "";
            do
            {
                t = scanner.Read();
                res += t.GetText() + " ";
            } while (t != Token.EOF);
            Console.WriteLine(res);
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
                Console.WriteLine("解释器运行时错误" + e.Message);
            }
            catch (ParseException e)
            {
                Console.WriteLine("词法解析时出错" + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("其他错误" + e.Message);
            }

            Console.ReadKey();
        }
    }
}
