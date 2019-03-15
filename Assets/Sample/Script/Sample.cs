/*
 * @Author: fasthro
 * @Date: 2019-02-27 18:05:23
 * @Description: 演示脚本
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConsoleFairyGUI;
using FairyGUI;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GRoot.inst.SetContentScaleFactor(2048, 1152, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
    }

    void Start()
    {
        Debug.Log("Sample Log");
        Debug.LogWarning("Sample LogWarning");
        Debug.LogError("Sample LogError");

        // 添加对象方法命令
        DebugConsole.AddCommand("command", "object command", "SampleCommand", typeof(Sample), this);
        // 添加静态方法命令
        DebugConsole.AddStaticCommand("staticCommand", "object command", "SampleStaticCommand", typeof(Sample));
    }

    public void SampleCommand()
    {
        Debug.Log("my name is object method command");
    }

    public static void SampleStaticCommand()
    {
        Debug.Log("my name is static method command");
    }
}
