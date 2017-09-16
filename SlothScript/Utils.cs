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
        public static void LogInfo(string msg)
        {
            Console.WriteLine("[SlothScript] - " + msg);
        }

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void LogInfo(string format, params object[] args)
        {
            Console.WriteLine("[SlothScript] - " + string.Format(format, args));
        }
    }
}
