/*
 * @Author: fasthro
 * @Date: 2019-02-27 18:05:23
 * @Description: 演示脚本
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace FastConsoleFairyGUI.Example
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
        }

        public void ExampleCommand()
        {
            Debug.Log("my name is object method command");
        }

        [MethodCommandAttribute("static_method", "static method")]
        public static void ExampleStaticCommand()
        {
            Debug.Log("my name is static method command");
        }
    }
}
