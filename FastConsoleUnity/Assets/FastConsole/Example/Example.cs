/*
 * @Author: fasthro
 * @Date: 2019-02-27 18:05:23
 * @Description: 演示脚本
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace FastConsole.Example
{
    public class Example : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            GRoot.inst.SetContentScaleFactor(2048, 1152, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            FastConsole.inst.Initialize(true);
        }

        void Start()
        {
            Debug.Log("Sample Log");
            Debug.LogWarning("Sample LogWarning");
            Debug.LogError("Sample LogError");

            // 添加对象方法命令
            FastConsole.inst.command.AddCommand("command", "object command", "ExampleCommand", typeof(Example), this);
            // 添加静态方法命令
            FastConsole.inst.command.AddCommand("staticCommand", "static command", "ExampleStaticCommand", typeof(Example));
        }

        public void ExampleCommand()
        {
            Debug.Log("my name is object method command");
        }

        public static void ExampleStaticCommand()
        {
            Debug.Log("my name is static method command");
        }
    }
}
