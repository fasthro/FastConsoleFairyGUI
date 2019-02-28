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

    private float temp = 0;
    private void Update()
    {
        temp += Time.deltaTime;
        if (temp > 0.1f)
        {
            var ran = Random.Range(1, 5);
            if (ran == 1)
            {
                // DebugConsole.Log("");
                Debug.Log("日\n志\n输\n出 : " + ran);
            }
            else if (ran == 3)
            {
                // DebugConsole.LogWarning("");
                Debug.LogWarning("日志输出 : " + ran);
            }
            else
            {
                // DebugConsole.LogError("");
                Debug.LogError("日志输出 : " + ran);
            }
            temp = 0;
        }
    }
}
