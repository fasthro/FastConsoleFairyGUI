/*
 * @Author: fasthro
 * @Date: 2019-12-07 11:09:53
 * @Description: 日志设置界面
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace FastConsoleFairyGUI
{
    public class UISetting : Window
    {
        #region component
        private GButton m_minimizeBtn;
        private GButton m_transparentBtn;
        private GButton m_touchBtn;
        private GButton m_singleShowBtn;
        private GButton m_systemInfoBtn;
        private GButton m_logcatBtn;
        private GButton m_btnClose;

        private GList m_colorList;
        private GImage[] m_colorImage;
        private GButton[] m_colorBtn;

        private Controller m_layoutController;
        private GButton m_layoutLeftTop;
        private GButton m_layoutLeftBottom;
        private GButton m_layoutRightTop;
        private GButton m_layoutRightBottom;
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
            m_minimizeBtn.selected = FastConsole.options.openState == OpenState.Minimize;

            m_transparentBtn = contentPane.GetChild("transparent_btn").asButton;
            m_transparentBtn.onClick.Set(OckTransparent);
            m_transparentBtn.selected = FastConsole.options.transparent;

            m_touchBtn = contentPane.GetChild("touch_btn").asButton;
            m_touchBtn.onClick.Set(OckTouch);
            m_touchBtn.selected = FastConsole.options.touchEnable;

            m_singleShowBtn = contentPane.GetChild("single_btn").asButton;
            m_singleShowBtn.onClick.Set(OckDetail);
            m_singleShowBtn.selected = FastConsole.options.detailEnable;

            m_systemInfoBtn = contentPane.GetChild("systeminfo_btn").asButton;
            m_systemInfoBtn.onClick.Set(OckSystemInfo);
            m_systemInfoBtn.selected = FastConsole.options.systemInfoEnable;

            m_btnClose = contentPane.GetChild("title").asCom.GetChild("close_btn").asButton;
            m_btnClose.onClick.Set(Hide);

            // list color
            m_colorList = contentPane.GetChild("color_list").asList;
            m_colorList.RemoveChildrenToPool();

            var colorItem = m_colorList.GetFromPool("").asCom;
            m_colorBtn = new GButton[m_maxColorCount];
            for (int i = 0; i < m_maxColorCount; i++)
            {
                var index = (i + 1).ToString();
                colorItem.GetChild("color_" + index).asImage.color = FastConsole.options.colors[i];
                m_colorBtn[i] = colorItem.GetChild("color_btn_" + index).asButton;
                m_colorBtn[i].data = i;
                m_colorBtn[i].onClick.Set(OckColor);

                if (FastConsole.options.colorIndex == i) m_colorBtn[i].selected = true;
                else m_colorBtn[i].selected = false;
            }
            m_colorList.AddChild(colorItem);

            m_layoutController = contentPane.GetController("layout");
            if (FastConsole.options.miniLayout == MiniLayout.LEFT_TOP) m_layoutController.SetSelectedIndex(0);
            else if (FastConsole.options.miniLayout == MiniLayout.RIGHT_TOP) m_layoutController.SetSelectedIndex(1);
            else if (FastConsole.options.miniLayout == MiniLayout.RIGHT_BOTTOM) m_layoutController.SetSelectedIndex(2);
            else if (FastConsole.options.miniLayout == MiniLayout.LEFT_BOTTOM) m_layoutController.SetSelectedIndex(3);

            m_layoutLeftTop = contentPane.GetChild("btn_left_top").asButton;
            m_layoutLeftTop.onClick.Set(OckLayoutLeftTop);

            m_layoutLeftBottom = contentPane.GetChild("btn_right_top").asButton;
            m_layoutLeftTop.onClick.Set(OckLayoutLeftBottom);

            m_layoutRightTop = contentPane.GetChild("btn_left_bottom").asButton;
            m_layoutLeftTop.onClick.Set(OckLayoutRightTop);

            m_layoutRightBottom = contentPane.GetChild("btn_right_bottom").asButton;
            m_layoutLeftTop.onClick.Set(OckLayoutRightBottom);
        }

        protected override void OnHide()
        {
            FastConsole.inst.mainUI.RefreshSetting();
        }

        private void OckMinimize() { FastConsole.options.openState = m_minimizeBtn.selected ? OpenState.Minimize : OpenState.Normal; }
        private void OckTransparent() { FastConsole.options.transparent = m_transparentBtn.selected; }
        private void OckTouch() { FastConsole.options.touchEnable = m_touchBtn.selected; }
        private void OckDetail() { FastConsole.options.detailEnable = m_singleShowBtn.selected; }
        private void OckSystemInfo() { FastConsole.options.systemInfoEnable = m_systemInfoBtn.selected; }
        private void OckColor(EventContext context)
        {
            var btn = context.sender as GButton;
            var index = (int)btn.data;

            if (FastConsole.options.colorIndex != index)
            {
                FastConsole.options.colorIndex = index;
                for (int i = 0; i < m_maxColorCount; i++)
                {
                    m_colorBtn[i].selected = FastConsole.options.colorIndex == i;
                }
            }
        }
        private void OckLayoutLeftTop()
        {
            Debug.Log("fffffff");
            FastConsole.options.miniLayout = MiniLayout.LEFT_TOP;
        }
        private void OckLayoutLeftBottom() { FastConsole.options.miniLayout = MiniLayout.LEFT_BOTTOM; }
        private void OckLayoutRightTop() { FastConsole.options.miniLayout = MiniLayout.RIGHT_TOP; }
        private void OckLayoutRightBottom() { FastConsole.options.miniLayout = MiniLayout.RIGHT_BOTTOM; }
    }
}
