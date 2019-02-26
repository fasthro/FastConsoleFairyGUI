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
        // 单一条条目选中显示日志详情
        public bool singleShow;
        // 接收Logcat日志
        public bool receivedLogcat;
    }
}
