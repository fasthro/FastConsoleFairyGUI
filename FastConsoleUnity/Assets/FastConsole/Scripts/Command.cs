/*
 * @Author: fasthro
 * @Date: 2019-03-14 18:32:29
 * @Description: 命令 
 * AddCommand 可添加对象方法和静态方法作为命令执行的函数.
 * Execute 执行命令
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FastConsole
{
    public class Command
    {
        class CommandInfo
        {
            // 方法信息
            public readonly MethodInfo method;
            // 命令描述
            public readonly string description;
            // 命令参数类型
            public readonly Type[] parameterTypes;
            // 方法所需对象
            public readonly object inst;

            public CommandInfo(MethodInfo method, object inst, string description)
            {
                this.method = method;
                this.inst = inst;
                this.description = description;
            }

            public bool IsValid()
            {
                if (!method.IsStatic && (inst == null || inst.Equals(null)))
                    return false;
                return true;
            }
        }

        // 所以命令
        private Dictionary<string, CommandInfo> commands;
        // 用户输入参数
        private List<string> arguments;
        // 特殊分割符号
        private string[] delimiters = new string[] { "\"\"", "{}", "[]", "()" };

        public Command()
        {
            commands = new Dictionary<string, CommandInfo>();
            arguments = new List<string>();
        }

        public void AddCommand(string cmd, string description, string methodName, Type type)
        {
            AddCommand(cmd, description, methodName, type, null);
        }

        public void AddCommand(string cmd, string description, string methodName, Type type, object inst)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method != null)
            {
                if (inst == null)
                {
                    if (!method.IsStatic)
                    {
                        Debug.LogError("The command : " + cmd + " not a static method");
                        return;
                    }
                }
                CommandInfo cmdInfo = new CommandInfo(method, inst, description);
                if (!commands.ContainsKey(cmd))
                {
                    commands.Add(cmd, cmdInfo);
                }
            }
        }

        public void Execute(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            input = input.Trim();

            arguments.Clear();

            int endIndex = IndexOf(input, ' ', 0);

            // 命令
            arguments.Add(Substring(input, 0, endIndex));

            // 参数
            for (int i = endIndex + 1; i < input.Length; i++)
            {
                var c = input[i];
                if (c == ' ')
                    continue;

                int dIndex = CheckDelimiter(c);
                if (dIndex == -1)
                {
                    endIndex = IndexOf(input, ' ', i + 1);
                }
                else
                {
                    endIndex = IndexOf(input, delimiters[dIndex][1], i + 1);
                }

                arguments.Add(Substring(input, i, endIndex - i));
                i = endIndex;
            }

            CommandInfo commandInfo = null;
            if (!commands.TryGetValue(arguments[0], out commandInfo))
            {
                Debug.LogError("Can't find command : " + arguments[0]);
                return;
            }
            else if (!commandInfo.IsValid())
            {
                Debug.LogError("The command : " + arguments[0] + "is valid");
                return;
            }

            if (string.IsNullOrEmpty(commandInfo.description))
            {
                Debug.Log("Execute the command : " + arguments[0]);
            }
            else
            {
                Debug.Log("Execute the command : " + arguments[0] + "(" + commandInfo.description + ")");
            }

            commandInfo.method.Invoke(commandInfo.inst, new object[] { });
        }

        private int IndexOf(string str, char separator, int startIndex)
        {
            int endIndex = str.IndexOf(separator, startIndex);
            if (endIndex == -1)
                return str.Length;
            return endIndex;
        }

        private int CheckDelimiter(char c)
        {
            for (int i = 0; i < delimiters.Length; i++)
            {
                if (c == delimiters[i][0])
                {
                    return i;
                }
            }
            return -1;
        }

        private string Substring(string str, int startIndex, int length)
        {
            return str.Substring(startIndex, length);
        }
    }
}
