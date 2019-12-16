/*
 * @Author: fasthro
 * @Date: 2019-03-14 18:32:29
 * @Description: 命令基类
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FastConsoleFairyGUI
{
    public abstract class Command
    {
        public string name { get; private set; }
        public string description { get; private set; }

        public Command(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public virtual void Execute(string line) { }

        public override string ToString()
        {
            return name;
        }

        protected void LogError(object message) { Debug.LogError("[FastConsole-Command] " + message.ToString()); }
    }
}
