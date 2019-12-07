/*
 * @Author: fasthro
 * @Date: 2019-12-07 11:09:53
 * @Description: 日志设置界面
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace FastConsole
{
    public class UIFastConsoleSetting : Window
    {
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

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject("FastConsole", "setting_panel").asCom;
            contentPane.MakeFullScreen();
            sortingOrder = int.MaxValue;
            gameObjectName = "Window - FastConsoleSetting";
        }

        protected override void OnShown()
        {
            m_minimizeBtn = contentPane.GetChild("minimize_btn").asButton;
            m_minimizeBtn.onClick.Set(OckMinimize);
            m_minimizeBtn.selected = FastConsoleSetting.data.defaultMinimize;

            m_transparentBtn = contentPane.GetChild("transparent_btn").asButton;
            m_transparentBtn.onClick.Set(OckTransparent);
            m_transparentBtn.selected = FastConsoleSetting.data.transparent;

            m_touchBtn = contentPane.GetChild("touch_btn").asButton;
            m_touchBtn.onClick.Set(OckTouch);
            m_touchBtn.selected = FastConsoleSetting.data.touch;

            m_singleShowBtn = contentPane.GetChild("single_btn").asButton;
            m_singleShowBtn.onClick.Set(OckSingle);
            m_singleShowBtn.selected = FastConsoleSetting.data.single;

            m_btnClose = contentPane.GetChild("title").asCom.GetChild("close_btn").asButton;
            m_btnClose.onClick.Set(OckClose);

            // list color
            m_colorList = contentPane.GetChild("color_list").asList;
            m_colorList.RemoveChildrenToPool();

            var colorItem = m_colorList.GetFromPool("").asCom;
            m_colorBtn = new GButton[m_maxColorCount];
            for (int i = 0; i < m_maxColorCount; i++)
            {
                var index = (i + 1).ToString();
                colorItem.GetChild("color_" + index).asImage.color = FastConsoleSetting.data.fontColor[i];
                m_colorBtn[i] = colorItem.GetChild("color_btn_" + index).asButton;
                m_colorBtn[i].data = i;
                m_colorBtn[i].onClick.Set(OckColor);

                if (FastConsoleSetting.data.fontColorIndex == i) m_colorBtn[i].selected = true;
                else m_colorBtn[i].selected = false;
            }
            m_colorList.AddChild(colorItem);
        }

        // 默认界面最小化显示
        private void OckMinimize() { FastConsoleSetting.data.defaultMinimize = m_minimizeBtn.selected; }

        // 透明模式
        private void OckTransparent() { FastConsoleSetting.data.transparent = m_transparentBtn.selected; }

        // 穿透模式
        private void OckTouch() { FastConsoleSetting.data.touch = m_touchBtn.selected; }

        // 设置单独界面展示日志详情
        private void OckSingle() { FastConsoleSetting.data.single = m_singleShowBtn.selected; }

        // 颜色选择
        private void OckColor(EventContext context)
        {
            var btn = context.sender as GButton;
            var index = (int)btn.data;

            if (FastConsoleSetting.data.fontColorIndex != index)
            {
                FastConsoleSetting.data.fontColorIndex = index;
                for (int i = 0; i < m_maxColorCount; i++)
                {
                    m_colorBtn[i].selected = FastConsoleSetting.data.fontColorIndex == i;
                }
            }
        }

        private void OckClose()
        {
            FastConsole.inst.mainUI.RefreshSetting();
            Hide();
        }
    }
}
