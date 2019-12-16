/*
 * @Author: fasthro
 * @Date: 2019-12-14 20:11:24
 * @Description: 函数命令 Attribute
 */
using System;

namespace FastConsoleFairyGUI
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class MethodCommandAttribute : Attribute
    {
        public readonly string name;           // 命令名称
        public readonly string description;    // 命令描述

        public MethodCommandAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}
