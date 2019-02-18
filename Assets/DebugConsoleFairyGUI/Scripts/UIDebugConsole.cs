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

        public void Refresh()
        {
            m_list.numItems = manager.logShowEntrys.Count;
        }
    }
}

