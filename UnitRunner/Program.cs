using System;

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

        static void SimplistUsage(string script)
        {
            // 最简用法
            var program = new SlothScript.Program(script);
            program.Compile();
            Console.WriteLine("ans=" + program.Run().returnVal);
        }

        static void CompleteUsage(string script)
        {
            // 详细用法
            var app = new SlothScript.Program(script);
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

        static int TestFoo(int a, int b)
        {
            return Math.Min(a, b);
        }

        static SlothScript.ExternalFunctionReturnValue SlothScriptExport_TestFoo(params object[] args)
        {
            return new SlothScript.ExternalFunctionReturnValue(TestFoo((int)args[0], (int)args[1]));
        }

        static void VmUsage()
        {
            var vm = new SlothScript.VM();
            // 添加函数库脚本到虚拟机
            var lib = new SlothScript.Program(@"
// 函数库
def max(x, y) do
  if x > y do
    return x;
  else
    return y;
  end;
end;
");
            lib.Compile();
            vm.AddScriptLib(lib);
            // 添加CS方法到虚拟机
            vm.AddCsharpMethod("cs_TestFoo", SlothScriptExport_TestFoo);
            // 在另外一个脚本中调用这两个方法
            var app = new SlothScript.Program(@"
// 测试函数库
a = 10;
b = cs_TestFoo(5,15); // csharp方法
return max(a, b);
");
            app.Compile();
            Console.WriteLine(app.Run(vm)); // 将vm作为参数传入，app即可使用vm的环境来运行
            Console.WriteLine(vm.CallScriptFunction("max", 22, 33)); // csharp调用脚本函数
        }

        /// <summary>
        /// 单元测试程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                VmUsage();
            }
            catch (Exception e)
            {
                Console.WriteLine("[UnitRunner] - " + e.Message);
            }

            Console.ReadKey();
        }
    }
}
