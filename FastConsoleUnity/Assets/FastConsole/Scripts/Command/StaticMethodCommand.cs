/*
 * @Author: fasthro
 * @Date: 2019-12-16 14:02:55
 * @Description: 静态函数函数命令
 */
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FastConsoleFairyGUI
{
    public class StaticMethodCommand : MethodCommand
    {
        public StaticMethodCommand(string name, string description, MethodInfo methodInfo) : base(name, description, methodInfo, null)
        {
        }
    }
}