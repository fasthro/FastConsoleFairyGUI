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
            m_singleShowBtn = contentPane.GetChild("single_show_btn").asButton;
            m_singleShowBtn.onClick.Set(OckSingle);

            m_logcatBtn = contentPane.GetChild("logcat_btn").asButton;
            m_logcatBtn.onClick.Set(OckLogcat);

            m_btnClose = contentPane.GetChild("title").asCom.GetChild("close_btn").asButton;
            m_btnClose.onClick.Set(OckClose);

            //TODO INIT
        }

        // 设置单独界面展示日志详情
        private void OckSingle()
        {
            manager.singleShow = m_singleShowBtn.selected;
        }

        // 设置接收Logcat日志
        private void OckLogcat()
        {
            manager.receivedLogcat = m_logcatBtn.selected;
        }

        private void OckClose()
        {
            Hide();
        }
    }
}
