/*
 * @Author: fasthro
 * @Date: 2019-12-16 14:02:55
 * @Description: 函数命令
 */
using System;
using System.Linq;
using System.Reflection;

namespace FastConsoleFairyGUI
{
    public class MethodCommand : Command
    {
        private object m_target;
        private MethodInfo m_methodInfo;
        private ParameterInfo[] m_parameters;
        private Type[] m_paramTypes;
        private object[] m_paramObj;

        /// <summary>
        /// 函数命令
        /// </summary>
        /// <param name="name">命令名称</param>
        /// <param name="description">命令描述</param>
        /// <param name="methodInfo">函数信息</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public MethodCommand(string name, string description, MethodInfo methodInfo, object target) : base(name, description)
        {
            m_target = target;
            m_methodInfo = methodInfo;
            m_parameters = methodInfo.GetParameters();
            m_paramTypes = m_parameters.Select(x => x.ParameterType).ToArray();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="line"></param>
        public override void Execute(string line)
        {
            var split = line.TrimEnd().Split(' ');
            if (split.Length - 1 < m_parameters.Length)
            {
                LogError("Parameter length error, should be " + m_parameters.Length + " parameters");
                return;
            }
            if (m_paramObj == null) m_paramObj = new object[m_paramTypes.Length];
            for (int i = 0; i < m_paramTypes.Length; i++)
            {
                try
                {
                    m_paramObj[i] = Convert.ChangeType(split[i + 1], m_paramTypes[i]);
                }
                catch (Exception e)
                {
                    LogError("The " + i + " parameter is wrong." + e.Message);
                }
                if (m_paramObj[i] == null) return;
            }
            m_methodInfo.Invoke(m_target, m_paramObj);
        }
    }
}