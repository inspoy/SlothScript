using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlothScript
{
    public enum ExternalFunctionReturnValueType
    {
        INT = 0,
        STRING = 1,
    }

    /// <summary>
    /// 外部函数的返回值统一类型
    /// </summary>
    public class ExternalFunctionReturnValue
    {
        public ExternalFunctionReturnValue(int val)
        {
            type = ExternalFunctionReturnValueType.INT;
            intVal = val;
        }
        public ExternalFunctionReturnValue(string val)
        {
            type = ExternalFunctionReturnValueType.STRING;
            strVal = val;
        }
        public ExternalFunctionReturnValueType type;
        public int intVal;
        public string strVal;
    }

    /// <summary>
    /// 外部函数委托
    /// </summary>
    /// <param name="args"></param>
    public delegate ExternalFunctionReturnValue ExternalFunction(params object[] args);

    /// <summary>
    /// 脚本虚拟机，整个运行时一般只需要一个实例
    /// </summary>
    public class VM
    {
        private Dictionary<string, AST.AstFuncDef> m_dictFunc;
        private Dictionary<string, ExternalFunction> m_dictCsFunc;

        public VM()
        {
            m_dictFunc = new Dictionary<string, AST.AstFuncDef>();
            m_dictCsFunc = new Dictionary<string, ExternalFunction>();
        }

        /// <summary>
        /// 从其他脚本程序中加载函数到虚拟机
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        public bool AddScriptLib(Program prog)
        {
            if (!prog.isCompiled)
            {
                // 未编译的脚本
                return false;
            }
            if (prog.mainEnv == null)
            {
                // 未运行的脚本，尝试运行
                if (prog.Run().success == false)
                {
                    // 运行失败
                    return false;
                }
            }
            // 添加函数到字典
            var allFunc = prog.mainEnv.GetAllFuncDef();
            m_dictFunc = m_dictFunc.Concat(allFunc).ToDictionary(kv => kv.Key, kv => kv.Value);
            return true;
        }

        internal AST.AstFuncDef GetFuncDef(string name)
        {
            if (m_dictFunc.ContainsKey(name))
            {
                return m_dictFunc[name];
            }
            else
            {
                return null;
            }
        }

        internal ExternalFunction GetCsFunc(string name)
        {
            if (m_dictCsFunc.ContainsKey(name))
            {
                return m_dictCsFunc[name];
            }
            return null;
        }

        /// <summary>
        /// 添加Csharp方法到虚拟机
        /// </summary>
        /// <param name="name">方法名</param>
        /// <param name="method">要添加的方法<see cref="ExternalFunction"/></param>
        public void AddCsharpMethod(string name, ExternalFunction method)
        {
            m_dictCsFunc[name] = method;
        }
    }
}
