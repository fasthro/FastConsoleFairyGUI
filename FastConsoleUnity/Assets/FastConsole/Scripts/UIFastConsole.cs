/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:56:42
 * @Description: 日志主界面
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace FastConsoleFairyGUI
{
    public class UIFastConsole : Window
    {
        #region component
        // 背景组件
        private GComponent m_bgCom;
        // 日志列表
        private GList m_list;
        // 功能按钮
        private GButton m_clearBtn;
        private GButton m_collapsedBtn;
        private GButton m_infoBtn;
        private GButton m_warnBtn;
        private GButton m_errorBtn;
        private GButton m_hideBtn;
        private GButton m_settingBtn;

        // 菜单操作按钮
        private GButton m_menuShowBtn;
        private GButton m_menuHideBtn;

        // 过滤组件
        private GTextInput m_filterInput;
        // 命令组件
        private GTextInput m_cmdInput;
        // 命令按钮
        private GButton m_cmdSendBtn;

        // 最小化组件
        private GComponent m_minCom;
        private GTextField m_minInfoCountText;
        private GTextField m_minWarnCountText;
        private GTextField m_minErrorCountText;
        private Controller m_windowController;

        // 透明控制器
        private Controller m_tpController;
        private Controller m_tpFliterController;
        private Controller m_tpCmdController;
        private Controller m_tpScrllBarController;
        private Controller m_tpMinimizeController;

        // 菜单控制器
        private Controller m_menuController;

        #endregion

        // 是否列表保持在最下面
        private bool m_listKeepDown = true;
        // 选中列表条目数据
        private FastConsoleEntry selectedData;
        // 最大化
        private bool m_maximizeShow;
        // 显示
        private bool m_isShow = false;

        protected override void OnInit()
        {
            contentPane = UIPackage.CreateObject("FastConsole", "panel").asCom;
            contentPane.MakeFullScreen();
            sortingOrder = int.MaxValue;
            gameObjectName = "Window - FastConsole";
        }

        protected override void OnShown()
        {
            m_isShow = true;
            m_bgCom = contentPane.GetChild("bg").asCom;
            m_windowController = contentPane.GetController("window");
            if (FastConsoleSetting.data.defaultMinimize)
            {
                m_maximizeShow = false;
                m_windowController.SetSelectedIndex(1);
            }
            else
            {
                m_maximizeShow = true;
                m_windowController.SetSelectedIndex(0);
            }

            m_menuController = contentPane.GetController("menu");
            m_menuController.SetSelectedIndex(0);

            m_list = contentPane.GetChild("list").asList;
            m_list.RemoveChildrenToPool();
            m_list.SetVirtual();
            m_list.itemRenderer = ItemRenderer;
            m_list.onClickItem.Set(OckClickItem);
            m_list.onTouchBegin.Set(OnListTouchBegin);
            m_list.scrollPane.onPullUpRelease.Set(OnListPullUpRelease);

            var bw = GRoot.inst.width / 7;

            m_clearBtn = contentPane.GetChild("clear_btn").asButton;
            m_clearBtn.onClick.Set(OckClear);
            SetBtnSize(m_clearBtn, bw, 1);

            m_collapsedBtn = contentPane.GetChild("collapse_btn").asButton;
            m_collapsedBtn.onClick.Set(OckCollapse);
            SetBtnSize(m_collapsedBtn, bw, 2);

            m_infoBtn = contentPane.GetChild("info_btn").asButton;
            m_infoBtn.onClick.Set(OckInfo);
            m_infoBtn.selected = true;
            SetBtnSize(m_infoBtn, bw, 3);

            m_warnBtn = contentPane.GetChild("warn_btn").asButton;
            m_warnBtn.onClick.Set(OckWarn);
            m_warnBtn.selected = true;
            SetBtnSize(m_warnBtn, bw, 4);

            m_errorBtn = contentPane.GetChild("error_btn").asButton;
            m_errorBtn.onClick.Set(OckError);
            m_errorBtn.selected = true;
            SetBtnSize(m_errorBtn, bw, 5);

            m_settingBtn = contentPane.GetChild("setting_btn").asButton;
            m_settingBtn.onClick.Set(OckSetting);
            SetBtnSize(m_settingBtn, bw, 6);

            m_hideBtn = contentPane.GetChild("hide_btn").asButton;
            m_hideBtn.onClick.Set(OckClose);
            SetBtnSize(m_hideBtn, bw, 7);

            // show menu
            m_menuShowBtn = contentPane.GetChild("menu_show_btn").asButton;
            m_menuShowBtn.onClick.Set(OckMenuShow);

            // hide menu
            m_menuHideBtn = contentPane.GetChild("menu_hide_btn").asButton;
            m_menuHideBtn.onClick.Set(OckMenuHide);

            // fliter
            var filterCom = contentPane.GetChild("fliter_com").asCom;
            m_filterInput = filterCom.GetChild("input_text").asTextInput;
            m_filterInput.onChanged.Set(OnFilterChange);

            // cmd
            var cmdCom = contentPane.GetChild("cmd_com").asCom;
            m_cmdInput = cmdCom.GetChild("input_text").asTextInput;
            m_cmdInput.onChanged.Set(OnCmdChange);
            m_cmdInput.onKeyDown.Set(OnEnterCmdSend);
            m_cmdSendBtn = cmdCom.GetChild("send_btn").asButton;
            m_cmdSendBtn.onClick.Set(OckCmdSend);
            m_cmdSendBtn.visible = false;

            // 最小化
            m_minCom = contentPane.GetChild("minimize_com").asCom;
            m_minCom.draggable = true;
            m_minCom.dragBounds = new Rect(0, 0, GRoot.inst.width, GRoot.inst.height);
            m_minCom.onClick.Set(OckMinimizeCom);

            m_tpMinimizeController = m_minCom.GetController("transparent");

            m_minInfoCountText = m_minCom.GetChild("info_text").asTextField;
            m_minWarnCountText = m_minCom.GetChild("warn_text").asTextField;
            m_minErrorCountText = m_minCom.GetChild("error_text").asTextField;

            // 透明模式控制
            m_tpController = contentPane.GetController("transparent");
            m_tpFliterController = filterCom.GetController("transparent");
            m_tpCmdController = cmdCom.GetController("transparent");
            m_tpScrllBarController = m_list.scrollPane.vtScrollBar.GetController("transparent");

            // 刷新设置
            RefreshSetting();
        }

        protected override void OnHide()
        {
            m_isShow = false;
        }

        public void Refresh()
        {
            if (!m_isShow)
                return;

            // count
            var infoCount = FastConsole.inst.infoCount.ToString();
            var warnCount = FastConsole.inst.warnCount.ToString();
            var errorCount = FastConsole.inst.errorCount.ToString();

            // 最大化的时候才更新列表显示
            if (m_maximizeShow)
            {
                m_list.numItems = FastConsole.inst.viewEntrys.Count;
                if (m_listKeepDown)
                {
                    m_list.scrollPane.ScrollBottom();
                }

                m_infoBtn.title = infoCount;
                m_warnBtn.title = warnCount;
                m_errorBtn.title = errorCount;
            }

            m_minInfoCountText.text = infoCount;
            m_minWarnCountText.text = warnCount;
            m_minErrorCountText.text = errorCount;
        }

        public void RefreshSetting()
        {
            if (!m_isShow)
                return;

            // 透明设置
            if (FastConsoleSetting.data.transparent)
            {
                m_tpController.SetSelectedIndex(1);
                m_tpFliterController.SetSelectedIndex(1);
                m_tpCmdController.SetSelectedIndex(1);
                m_tpScrllBarController.SetSelectedIndex(1);
                m_tpMinimizeController.SetSelectedIndex(1);
            }
            else
            {
                m_tpController.SetSelectedIndex(0);
                m_tpFliterController.SetSelectedIndex(0);
                m_tpCmdController.SetSelectedIndex(0);
                m_tpScrllBarController.SetSelectedIndex(0);
                m_tpMinimizeController.SetSelectedIndex(0);
            }

            // 穿透设置
            var touchable = !FastConsoleSetting.data.touch;
            m_list.touchable = touchable;
            m_bgCom.touchable = touchable;

            Refresh();
        }

        // 设置功能按钮Size
        private void SetBtnSize(GButton button, float width, int index)
        {
            button.width = width;
            button.x = width * (index - 1);
        }

        // 重置
        private void Reset()
        {
            selectedData = null;
        }

        private void ItemRenderer(int index, GObject obj)
        {
            var item = obj.asCom;
            var data = FastConsole.inst.viewEntrys[index];

            var colorController = item.GetController("color");
            var typeController = item.GetController("type");
            var groupController = item.GetController("group");
            var selectedController = item.GetController("selected");
            var transparentController = item.GetController("transparent");

            var contentText = item.GetChild("content_text").asRichTextField;
            var countText = item.GetChild("count_text").asTextField;

            var copyBtn = item.GetChild("btn_copy").asButton;

            // type
            if (data.logType == ConsoleLogType.Log) typeController.SetSelectedIndex(0);
            else if (data.logType == ConsoleLogType.Warning) typeController.SetSelectedIndex(1);
            else if (data.logType == ConsoleLogType.Error) typeController.SetSelectedIndex(2);

            // color
            if (index % 2 == 0) colorController.SetSelectedIndex(0);
            else colorController.SetSelectedIndex(1);

            // group
            if (data.logCount > 1) groupController.SetSelectedIndex(0);
            else groupController.SetSelectedIndex(1);

            // selected
            if (data.selected) selectedController.SetSelectedIndex(1);
            else selectedController.SetSelectedIndex(0);

            // transparent
            if (FastConsoleSetting.data.transparent) transparentController.SetSelectedIndex(1);
            else transparentController.SetSelectedIndex(0);

            // count
            if (data.logCount > 999) countText.text = "999+";
            else countText.text = data.logCount.ToString();

            if (!FastConsoleSetting.data.single && data.selected)
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

            // 字体颜色设置
            contentText.color = FastConsoleSetting.data.fontColor[FastConsoleSetting.data.fontColorIndex];

            item.data = data;
        }

        private void OckClickItem(EventContext context)
        {
            GComponent item = context.data as GComponent;
            FastConsoleEntry data = item.data as FastConsoleEntry;

            // 显示日志详情界面
            if (FastConsoleSetting.data.single)
            {
                FastConsole.inst.detailUI.Show();
                FastConsole.inst.detailUI.Refresh(data);
            }

            // 选中状态设置
            if (selectedData != null)
            {
                if (selectedData == data) selectedData.selected = !selectedData.selected;
                else
                {
                    selectedData.selected = false;
                    data.selected = true;
                    selectedData = data;
                }
            }
            else
            {
                selectedData = data;
                selectedData.selected = true;
            }

            Refresh();

            m_listKeepDown = false;
        }

        private void OckClickItemCopy(EventContext context)
        {
            GComponent item = context.sender as GComponent;
            FastConsoleEntry data = item.data as FastConsoleEntry;
            FastConsole.inst.native.Copy(data.ToString());
        }

        private void OnListTouchBegin() { m_listKeepDown = false; }
        private void OnListPullUpRelease() { m_listKeepDown = true; }

        private void OnFilterChange()
        {
            Reset();
            FastConsole.inst.Filter(m_filterInput.text);
        }

        private void OckClear()
        {
            Reset();
            FastConsole.inst.Clear();
        }

        private void OckCollapse()
        {
            Reset();
            FastConsole.inst.SetCollapsed(m_collapsedBtn.selected);
        }

        private void OckInfo()
        {
            Reset();
            FastConsole.inst.SetInfoSelected(m_infoBtn.selected);
        }

        private void OckWarn()
        {
            Reset();
            FastConsole.inst.SetWarnSelected(m_warnBtn.selected);
        }

        private void OckError()
        {
            Reset();
            FastConsole.inst.SetErrorSelected(m_errorBtn.selected);
        }

        private void OckSetting() { FastConsole.inst.settingUI.Show(); }
        private void OckMenuShow() { m_menuController.SetSelectedIndex(0); }
        private void OckMenuHide() { m_menuController.SetSelectedIndex(1); }

        private void OckMinimizeCom()
        {
            m_windowController.SetSelectedIndex(0);
            m_maximizeShow = true;
            Refresh();
        }

        private void OnCmdChange()
        {
            string cmd = m_cmdInput.text;
            if (string.IsNullOrEmpty(cmd)) m_cmdSendBtn.visible = false;
            else m_cmdSendBtn.visible = true;
        }

        private void OnEnterCmdSend()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                OckCmdSend();
        }

        private void OckCmdSend()
        {
            string cmd = m_cmdInput.text;
            if (!string.IsNullOrEmpty(cmd))
                FastConsole.inst.command.Execute(cmd);
            m_cmdInput.text = "";
            m_cmdSendBtn.visible = false;
        }

        private void OckClose()
        {
            m_maximizeShow = false;
            m_windowController.SetSelectedIndex(1);
        }
    }
}

