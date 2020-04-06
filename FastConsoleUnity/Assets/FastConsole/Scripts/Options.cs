/*
 * @Author: fasthro
 * @Date: 2019-02-26 15:42:56
 * @Description: DebugConsole Setting
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastConsoleFairyGUI
{
    /// <summary>
    /// 打开状态
    /// </summary>
    public enum OpenState
    {
        Normal,
        Minimize,
    }

    /// <summary>
    /// 迷你界面布局
    /// </summary>
    public enum MiniLayout
    {
        LEFT_TOP,
        LEFT_BOTTOM,
        RIGHT_TOP,
        RIGHT_BOTTOM,
    }

    [System.Serializable]
    public class Options : ScriptableObject
    {
        public OpenState openState = OpenState.Minimize;  // 打开状态
        public MiniLayout miniLayout = MiniLayout.LEFT_TOP;
        public bool transparent;                          // 透明模式
        public bool touchEnable;                          // 是否接受输入事件
        public bool detailEnable;                         // 是否启用日志详情界面
        public bool systemInfoEnable = true;              // 是否启用系统信息
        public Color32[] colors = new Color32[]
        {
            new Color32(0xa7, 0xa7, 0xa7, 0xff),
            new Color32(0xff, 0xff, 0xff, 0xff),
            new Color32(0xff, 0xbd, 0x17, 0xff),
            new Color32(0xF3, 0x41, 0x50, 0xff),
            new Color32(0xA8, 0x39, 0xBA, 0xff),
            new Color32(0xFF, 0x92, 0x3A, 0xff),
            new Color32(0x02, 0xBD, 0x5B, 0xff),
            new Color32(0x37, 0x47, 0x55, 0xff),
            new Color32(0x1F, 0x9F, 0xC2, 0xff),
            new Color32(0x07, 0x8C, 0xE4, 0xff),
            new Color32(0x1D, 0x1D, 0x1D, 0xff),
            new Color32(0x02, 0xBC, 0xBD, 0xff)
        };

        public int colorIndex { get; set; }
        public Color32 GetColor() { return colors[colorIndex]; }
    }
}
