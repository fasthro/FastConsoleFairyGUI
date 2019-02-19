/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:56:42
 * @Description: DebugConsoleFairyGUI 日志主界面
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace DebugConsoleFairyGUI
{
    public class UIDebugConsole : Window
    {
        private DebugConsole manager;
        private GList m_list;
        private GButton m_clearBtn;
        private GButton m_collapsedBtn;
        private GButton m_infoBtn;
        private GButton m_warnBtn;
        private GButton m_errorBtn;
        private GButton m_logcatgBtn;
        private GButton m_hideBtn;
        private GButton m_settingBtn;

        public UIDebugConsole(DebugConsole mgr)
        {
            manager = mgr;
        }

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject(LogConst.UI_PACKAGE_NAME, "panel").asCom;
            contentPane.SetSize(GRoot.inst.width, GRoot.inst.height);
        }

        protected override void OnShown()
        {
            m_list = contentPane.GetChild("list").asList;
            m_list.RemoveChildrenToPool();
            m_list.SetVirtual();
            m_list.itemRenderer = ItemRenderer;

            m_clearBtn = contentPane.GetChild("clear_btn").asButton;
            m_clearBtn.onClick.Set(OckClear);

            m_collapsedBtn = contentPane.GetChild("collapse_btn").asButton;
            m_collapsedBtn.selected = manager.collapsed;
            m_collapsedBtn.onClick.Set(OckCollapse);

            m_infoBtn = contentPane.GetChild("info_btn").asButton;
            m_infoBtn.selected = manager.infoSelected;
            m_infoBtn.onClick.Set(OckInfo);

            m_warnBtn = contentPane.GetChild("warn_btn").asButton;
            //m_warnBtn.selected = manager.warnSelected;
            m_warnBtn.onClick.Set(OckWarn);

            m_errorBtn = contentPane.GetChild("error_btn").asButton;
            //m_errorBtn.selected = manager.errorSelected;
            m_errorBtn.onClick.Set(OckError);
            
            m_logcatgBtn = contentPane.GetChild("logcat_btn").asButton;
            //m_logcatgBtn.selected = false;
            m_logcatgBtn.onClick.Set(OckLogcat);

            m_settingBtn = contentPane.GetChild("setting_btn").asButton;
            m_settingBtn.onClick.Set(OckSetting);

            m_hideBtn = contentPane.GetChild("hide_btn").asButton;
            m_hideBtn.onClick.Set(OckClose);
        }

        private void ItemRenderer(int index, GObject obj)
        {
            var item = obj.asCom;
            var data = manager.logShowEntrys[index];

            var colorController = item.GetController("color");
            var typeController = item.GetController("type");
            var contentText = item.GetChild("content_text").asRichTextField;

            if (data.logType == LogType.Log)
            {
                typeController.SetSelectedIndex(0);
            }
            else if (data.logType == LogType.Warning)
            {
                typeController.SetSelectedIndex(1);
            }
            else if (data.logType == LogType.Error)
            {
                typeController.SetSelectedIndex(2);
            }

            if (index % 2 == 0)
            {
                colorController.SetSelectedIndex(0);
            }
            else
            {
                colorController.SetSelectedIndex(1);
            }

            contentText.text = data.logContent;
        }

        private void OckClear()
        {
            manager.ClearLog();
        }

        private void OckCollapse()
        {
            manager.SetCollapsed(m_collapsedBtn.selected);
        }

        private void OckInfo()
        {
            Debug.Log(m_infoBtn.selected);
            manager.SetInfo(m_infoBtn.selected);
        }

        private void OckWarn()
        {
            manager.SetWarn(m_warnBtn.selected);
        }

        private void OckError()
        {
            manager.SetError(m_errorBtn.selected);
        }

        private void OckLogcat()
        {
            
        }

        private void OckSetting()
        {

        }

        private void OckClose()
        {

        }

        public void Refresh()
        {
            m_list.numItems = manager.logShowEntrys.Count;
        }
    }
}

