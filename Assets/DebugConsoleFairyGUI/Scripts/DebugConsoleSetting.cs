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
        // 默认最小化显示界面
        public bool defaultMinimize;
        // 开启透明模式
        public bool transparent;
        // 开启穿透点击模式
        public bool touch;
        // 单一条条目选中显示日志详情
        public bool single;
        // 当前列表字体颜色
        public int listFontColorIndex = 0;

        
        #region 列表字体颜色

        [HideInInspector]
        public Color32 listFontColor;

        public static Color32[] LIST_FONT_COLOR = new Color32[]
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

        #endregion
    }
}
