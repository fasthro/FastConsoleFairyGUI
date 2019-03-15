using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace DebugConsoleFairyGUI
{
    public class UIDebugConsoleSetting : Window
    {
        // DebugConsole 实例
        private DebugConsole manager;

        #region component
        private GButton m_minimizeBtn;
        private GButton m_transparentBtn;
        private GButton m_touchBtn;
        private GButton m_singleShowBtn;
        private GButton m_logcatBtn;
        private GButton m_btnClose;

        private GList m_colorList;
        private GImage[] m_colorImage;
        private GButton[] m_colorBtn;
        #endregion

        // 颜色最大数量
        private int m_maxColorCount = 12;

        public UIDebugConsoleSetting(DebugConsole mgr)
        {
            manager = mgr;
        }

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject(LogConst.UI_PACKAGE_NAME, "setting_panel").asCom;
            contentPane.MakeFullScreen();
            sortingOrder = manager.sortingOrder;
            gameObjectName = "Window - DebugConsoleSetting";
        }

        protected override void OnShown()
        {
            m_minimizeBtn = contentPane.GetChild("minimize_btn").asButton;
            m_minimizeBtn.onClick.Set(OckMinimize);
            m_minimizeBtn.selected = manager.settingConfig.defaultMinimize;

            m_transparentBtn = contentPane.GetChild("transparent_btn").asButton;
            m_transparentBtn.onClick.Set(OckTransparent);
            m_transparentBtn.selected = manager.settingConfig.transparent;

            m_touchBtn = contentPane.GetChild("touch_btn").asButton;
            m_touchBtn.onClick.Set(OckTouch);
            m_touchBtn.selected = manager.settingConfig.touch;

            m_singleShowBtn = contentPane.GetChild("single_btn").asButton;
            m_singleShowBtn.onClick.Set(OckSingle);
            m_singleShowBtn.selected = manager.settingConfig.single;

            m_btnClose = contentPane.GetChild("title").asCom.GetChild("close_btn").asButton;
            m_btnClose.onClick.Set(OckClose);

            // list color
            m_colorList = contentPane.GetChild("color_list").asList;
            m_colorList.RemoveChildrenToPool();

            var colorItem = m_colorList.GetFromPool("").asCom;
            m_colorImage = new GImage[m_maxColorCount];
            m_colorBtn = new GButton[m_maxColorCount];
            for (int i = 0; i < m_maxColorCount; i++)
            {
                var index = (i + 1).ToString();
                m_colorImage[i] = colorItem.GetChild("color_" + index).asImage;
                m_colorBtn[i] = colorItem.GetChild("color_btn_" + index).asButton;
                m_colorBtn[i].data = i;
                m_colorBtn[i].onClick.Set(OckColor);

                if (manager.settingConfig.listFontColorIndex == i)
                {
                    m_colorBtn[i].selected = true;
                }
                else
                {
                    m_colorBtn[i].selected = false;
                }
            }
            m_colorList.AddChild(colorItem);
        }

        // 默认界面最小化显示
        private void OckMinimize()
        {
            manager.settingConfig.defaultMinimize = m_minimizeBtn.selected;
        }

        // 透明模式
        private void OckTransparent()
        {
            manager.settingConfig.transparent = m_transparentBtn.selected;
        }

        // 穿透模式
        private void OckTouch()
        {
            manager.settingConfig.touch = m_touchBtn.selected;
        }

        // 设置单独界面展示日志详情
        private void OckSingle()
        {
            manager.settingConfig.single = m_singleShowBtn.selected;
        }

        // 颜色选择
        private void OckColor(EventContext context)
        {
            var btn = context.sender as GButton;
            var index = (int)btn.data;

            if (manager.settingConfig.listFontColorIndex != index)
            {
                manager.settingConfig.listFontColorIndex = index;
                manager.settingConfig.listFontColor = DebugConsoleSetting.LIST_FONT_COLOR[index];

                for (int i = 0; i < m_maxColorCount; i++)
                {
                    if (manager.settingConfig.listFontColorIndex == i)
                    {
                        m_colorBtn[i].selected = true;
                    }
                    else
                    {
                        m_colorBtn[i].selected = false;
                    }
                }
            }
        }

        private void OckClose()
        {
            manager.mainUI.RefreshSetting();

            Hide();
        }
    }
}
