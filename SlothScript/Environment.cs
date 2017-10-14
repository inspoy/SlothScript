using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlothScript
{
    public class Environment
    {
        Dictionary<string, EvalValue> m_hashMap;

        public Environment()
        {
            m_hashMap = new Dictionary<string, EvalValue>();
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
    }
}
