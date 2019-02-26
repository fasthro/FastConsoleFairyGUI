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
        private GButton m_transparentBtn;
        private GButton m_touchBtn;
        private GButton m_singleShowBtn;
        private GButton m_logcatBtn;
        private GButton m_btnClose;
        #endregion

        public UIDebugConsoleSetting(DebugConsole mgr)
        {
            manager = mgr;
        }

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject(LogConst.UI_PACKAGE_NAME, "setting_panel").asCom;
            contentPane.SetSize(GRoot.inst.width, GRoot.inst.height);
        }

        protected override void OnShown()
        {
            m_transparentBtn = contentPane.GetChild("transparent_btn").asButton;
            m_transparentBtn.onClick.Set(OckTransparent);
            m_transparentBtn.selected = manager.settingConfig.transparent;
            
            m_touchBtn = contentPane.GetChild("touch_btn").asButton;
            m_touchBtn.onClick.Set(OckTouch);
            m_touchBtn.selected = manager.settingConfig.touch;
            
            m_singleShowBtn = contentPane.GetChild("single_btn").asButton;
            m_singleShowBtn.onClick.Set(OckSingle);
            m_singleShowBtn.selected = manager.settingConfig.single;

            m_logcatBtn = contentPane.GetChild("logcat_btn").asButton;
            m_logcatBtn.onClick.Set(OckLogcat);
            m_logcatBtn.selected = manager.settingConfig.logcat;

            m_btnClose = contentPane.GetChild("title").asCom.GetChild("close_btn").asButton;
            m_btnClose.onClick.Set(OckClose);
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

        // 设置接收Logcat日志
        private void OckLogcat()
        {
            manager.settingConfig.logcat = m_logcatBtn.selected;
        }

        private void OckClose()
        {
            manager.mainUI.RefreshSetting();
            
            Hide();
        }
    }
}
