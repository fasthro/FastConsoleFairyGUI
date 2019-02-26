/*
 * @Author: fasthro
 * @Date: 2019-02-26 15:42:56
 * @Description: DebugConsole 设置配置
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugConsoleFairyGUI
{
    [System.Serializable]
    public class DebugConsoleSetting : ScriptableObject
    {
        // 开启透明模式
        public bool transparent;
        // 开启穿透点击模式
        public bool touch;
        // 单一条条目选中显示日志详情
        public bool single;
        // 接收Logcat日志
        public bool logcat;
    }
}
