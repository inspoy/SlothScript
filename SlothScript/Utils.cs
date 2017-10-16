using System;
using System.Collections.Generic;
using System.Text;

namespace SlothScript
{
    /// <summary>
    /// 内部实用工具，以静态类为主
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="msg"></param>
        public static void LogDebug(string msg)
        {
            // Console.WriteLine("[SlothScript] - " + msg);
        }

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void LogDebug(string format, params object[] args)
        {
            // Console.WriteLine("[SlothScript] - " + string.Format(format, args));
        }

        public static string GetScopeString()
        {
            StringBuilder sb = new StringBuilder();
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 32; ++i)
            {
                sb.Append((char)(rand.Next(65, 90)));
            }
            return sb.ToString();
        }
    }
}
