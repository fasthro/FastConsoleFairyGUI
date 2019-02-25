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
        // DebugConsole 实例
        private DebugConsole manager;

        // 日志详情界面
        private UIDebugConsoleDetail m_detailUI;

        #region component

        private GList m_list;
        private GButton m_clearBtn;
        private GButton m_collapsedBtn;
        private GButton m_infoBtn;
        private GButton m_warnBtn;
        private GButton m_errorBtn;
        private GButton m_logcatgBtn;
        private GButton m_hideBtn;
        private GButton m_settingBtn;

        #endregion

        // 是否列表保持在最下面
        private bool m_keepBottom;
        // 当前选中的数据
        private LogEntry selectedData;

        public UIDebugConsole(DebugConsole mgr)
        {
            manager = mgr;
        }

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject(LogConst.UI_PACKAGE_NAME, "panel").asCom;
            contentPane.SetSize(GRoot.inst.width, GRoot.inst.height);

            m_keepBottom = true;
        }

        protected override void OnShown()
        {
            m_list = contentPane.GetChild("list").asList;
            m_list.RemoveChildrenToPool();
            m_list.SetVirtual();
            m_list.itemRenderer = ItemRenderer;
            m_list.onClickItem.Set(OckClickItem);
            m_list.onTouchBegin.Set(OnTouchBegin);
            m_list.scrollPane.onPullUpRelease.Set(OnPullUpRelease);

            var bw = GRoot.inst.width / 8;

            m_clearBtn = contentPane.GetChild("clear_btn").asButton;
            m_clearBtn.onClick.Set(OckClear);
            SetBtnSize(m_clearBtn, bw, 1);

            m_collapsedBtn = contentPane.GetChild("collapse_btn").asButton;
            m_collapsedBtn.onClick.Set(OckCollapse);
            SetBtnSize(m_collapsedBtn, bw, 2);

            m_infoBtn = contentPane.GetChild("info_btn").asButton;
            m_infoBtn.onClick.Set(OckInfo);
            SetBtnSize(m_infoBtn, bw, 3);

            m_warnBtn = contentPane.GetChild("warn_btn").asButton;
            m_warnBtn.onClick.Set(OckWarn);
            SetBtnSize(m_warnBtn, bw, 4);

            m_errorBtn = contentPane.GetChild("error_btn").asButton;
            m_errorBtn.onClick.Set(OckError);
            SetBtnSize(m_errorBtn, bw, 5);

            m_logcatgBtn = contentPane.GetChild("logcat_btn").asButton;
            m_logcatgBtn.onClick.Set(OckLogcat);
            SetBtnSize(m_logcatgBtn, bw, 6);

            m_settingBtn = contentPane.GetChild("setting_btn").asButton;
            m_settingBtn.onClick.Set(OckSetting);
            SetBtnSize(m_settingBtn, bw, 7);

            m_hideBtn = contentPane.GetChild("hide_btn").asButton;
            m_hideBtn.onClick.Set(OckClose);
            SetBtnSize(m_hideBtn, bw, 8);
        }

        // 设置功能按钮Size
        private void SetBtnSize(GButton button, float width, int index)
        {
            button.width = width;
            button.x = width * (index - 1);
        }

        private void ItemRenderer(int index, GObject obj)
        {
            var item = obj.asCom;
            var data = manager.logShowEntrys[index];

            var colorController = item.GetController("color");
            var typeController = item.GetController("type");
            var groupController = item.GetController("group");
            var selectedController = item.GetController("selected");

            var contentText = item.GetChild("content_text").asRichTextField;
            var countText = item.GetChild("count_text").asTextField;

            var copyBtn = item.GetChild("btn_copy").asButton;

            // type
            if (data.logType == DebugLogType.Log)
            {
                typeController.SetSelectedIndex(0);
            }
            else if (data.logType == DebugLogType.Warning)
            {
                typeController.SetSelectedIndex(1);
            }
            else if (data.logType == DebugLogType.Error)
            {
                typeController.SetSelectedIndex(2);
            }

            // color
            if (index % 2 == 0)
            {
                colorController.SetSelectedIndex(0);
            }
            else
            {
                colorController.SetSelectedIndex(1);
            }

            // group
            if (data.logCount > 1)
            {
                groupController.SetSelectedIndex(0);
            }
            else
            {
                groupController.SetSelectedIndex(1);
            }

            // selected
            if (data.selected)
            {
                selectedController.SetSelectedIndex(1);
            }
            else
            {
                selectedController.SetSelectedIndex(0);
            }

            // count
            if (data.logCount > 999)
            {
                countText.text = "999+";
            }
            else
            {
                countText.text = data.logCount.ToString();
            }

            if (manager.single && data.selected)
            {
                contentText.autoSize = AutoSizeType.Height;
                contentText.text = data.ToString();

                item.height = contentText.textHeight;

                copyBtn.visible = true;
                copyBtn.data = data;
                copyBtn.onClick.Set(OckClickItemCopy);
            }
            else
            {
                contentText.autoSize = AutoSizeType.None;
                contentText.text = data.logContent;
                contentText.height = contentText.initHeight;

                copyBtn.visible = false;

                item.height = item.initHeight;
            }

            item.data = data;
        }

        private void OckClickItem(EventContext context)
        {
            GComponent item = context.data as GComponent;
            LogEntry data = item.data as LogEntry;

            // 显示日志详情界面
            if (!manager.single)
            {
                if (m_detailUI == null)
                {
                    m_detailUI = new UIDebugConsoleDetail();
                    m_detailUI.Show();
                    m_detailUI.Refresh(data);
                }
                else
                {
                    m_detailUI.Show();
                    m_detailUI.Refresh(data);
                }
            }

            // 选中状态设置
            if (selectedData != null)
            {
                selectedData.selected = false;
            }
            selectedData = data;
            selectedData.selected = true;

            Refresh();

            // 取消列表自动向下显示
            m_keepBottom = false;
        }

        private void OckClickItemCopy(EventContext context)
        {
            GComponent item = context.sender as GComponent;
            LogEntry data = item.data as LogEntry;

            // TODO COPY
        }

        private void OnTouchBegin()
        {
            m_keepBottom = false;
        }

        private void OnPullUpRelease()
        {
            m_keepBottom = true;
        }

        public void Refresh()
        {
            m_list.numItems = manager.logShowEntrys.Count;
            if (m_keepBottom)
            {
                m_list.scrollPane.ScrollBottom();
            }

            // count
            m_infoBtn.title = manager.infoCount.ToString();
            m_warnBtn.title = manager.warnCount.ToString();
            m_errorBtn.title = manager.errorCount.ToString();
        }

        // 重置
        private void Reset()
        {
            selectedData = null;
        }

        private void OckClear()
        {
            Reset();

            manager.ClearLog();
        }

        private void OckCollapse()
        {
            Reset();

            manager.SetCollapsed(m_collapsedBtn.selected);
        }

        private void OckInfo()
        {
            Reset();

            manager.SetInfo(m_infoBtn.selected);
        }

        private void OckWarn()
        {
            Reset();

            manager.SetWarn(m_warnBtn.selected);
        }

        private void OckError()
        {
            Reset();

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
    }
}

