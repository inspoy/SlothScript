using System;
using System.Collections.Generic;
using SlothScript.AST;

namespace SlothScript
{
    internal interface IEnvironment
    {
        void Set(string name, EvalValue content);
        EvalValue Get(string name);
        string GetScopeName();
        MainEnvironment GetMainEnv();
    }

    internal class MainEnvironment : IEnvironment
    {
        Dictionary<string, EvalValue> m_hashMap;
        private Dictionary<string, AstFuncDef> m_dictFunctions;
        VM m_vm;

        public MainEnvironment(VM vm = null)
        {
            m_hashMap = new Dictionary<string, EvalValue>();
            m_dictFunctions = new Dictionary<string, AstFuncDef>();
            m_vm = vm;
        }

        /// <summary>
        /// 设置变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void Set(string name, EvalValue content)
        {
            m_hashMap[name] = content;
        }

        /// <summary>
        /// 读取变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EvalValue Get(string name)
        {
            if (m_hashMap.ContainsKey(name))
            {
                return m_hashMap[name];
            }
            else
            {
                return null;
            }
        }

        public string GetScopeName()
        {
            return "Main";
        }

        public ExternalFunction GetCsFuncDef(string name)
        {
            if (m_vm != null && m_vm.GetCsFunc(name) != null)
            {
                return m_vm.GetCsFunc(name);
            }
            return null;
        }

        public AstFuncDef GetFuncDef(string name)
        {
            // 优先自己定义的
            if (m_dictFunctions.ContainsKey(name))
            {
                return m_dictFunctions[name];
            }
            // 没有的话找虚拟机中其他库定义的
            if (m_vm != null && m_vm.GetFuncDef(name) != null)
            {
                return m_vm.GetFuncDef(name);
            }
            return null;
        }

        public void SetFuncDef(AstFuncDef func)
        {
            if (m_dictFunctions.ContainsKey(func.name))
            {
                throw new RunTimeException("函数重复定义:" + func.name);
            }
            m_dictFunctions[func.name] = func;
        }

        public Dictionary<string, AstFuncDef> GetAllFuncDef()
        {
            return m_dictFunctions;
        }

        public MainEnvironment GetMainEnv()
        {
            return this;
        }
    }

    /// <summary>
    /// 函数作用域
    /// 调用函数时创建
    /// </summary>
    internal class FuncEnvironment : IEnvironment
    {
        Dictionary<string, EvalValue> m_hashMap;
        IEnvironment m_parent;
        public IEnvironment parent { get => m_parent; }
        string m_name;
        MainEnvironment m_mainEnv;

        public FuncEnvironment(IEnvironment parent, string name)
        {
            m_hashMap = new Dictionary<string, EvalValue>();
            m_parent = parent;
            m_name = name;
            m_mainEnv = parent.GetMainEnv();
        }

        public EvalValue Get(string name)
        {
            if (m_hashMap.ContainsKey(name))
            {
                return m_hashMap[name];
            }
            return m_parent.Get(name);
        }

        public void Set(string name, EvalValue content)
        {
            if (m_hashMap.ContainsKey(name))
            {
                m_hashMap[name] = content;
            }
            else
            {
                m_parent.Set(name, content);
            }
        }

        public void AddArg(string name, EvalValue content)
        {
            m_hashMap[name] = content;
        }

        public string GetScopeName()
        {
            return m_name;
        }

        public MainEnvironment GetMainEnv()
        {
            return m_mainEnv;
        }
    }
}
