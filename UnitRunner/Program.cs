﻿using System;

namespace UnitRunner
{
    /// <summary>
    /// 主程序
    /// </summary>
    class Program
    {
        private static string TEST_SCRIPT1 = @"
sum = 0;
i = 1;
while i <= 1000000 do
  if i % 2 == 0 do
    sum = sum + i; // 计算2+4+...+100
  end;
  i = i + 1; // 循环计数器
end;
return sum; // 返回结果
";
        private static string TEST_SCRIPT2 = @"
sum = """";
i = 1;
while i <= 5 do
  sum = sum + i;
  i = i + 1;
end;
return sum;
";
        private static string TEST_SCRIPT3 = @"
def fib(n) do
  if (n == 0)||(n == 1) do
    return 1;
  end;
  return fib(n-2) + fib(n-1);
end;
return fib(10);
";

        /// <summary>
        /// 单元测试程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                //SlothScript.Program.TestParser(TEST_SCRIPT3);
                //SlothScript.Program.TestParser(TEST_SCRIPT2);

                // 最简用法
                //var program = new SlothScript.Program(TEST_SCRIPT2);
                //program.Compile();
                //Console.WriteLine("ans=" + program.Run().returnVal);
                //throw new Exception("BREAK");

                // 详细用法
                var app = new SlothScript.Program(TEST_SCRIPT3);
                var compileResult = app.Compile();
                if (compileResult.success)
                {
                    var startTime = DateTime.Now;
                    var runResult = app.Run();
                    var endTime = DateTime.Now;
                    Console.WriteLine("耗时:{0}ms", (endTime - startTime).TotalMilliseconds);
                    if (runResult.success)
                    {
                        Console.WriteLine("返回值 => " + runResult.returnVal);
                    }
                    else
                    {
                        Console.WriteLine("错误信息 => " + runResult.error);
                    }
                }
                else
                {
                    Console.WriteLine("编译错误 => " + compileResult.output);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[UnitRunner] - " + e.Message);
            }

            Console.ReadKey();
        }
    }
}
