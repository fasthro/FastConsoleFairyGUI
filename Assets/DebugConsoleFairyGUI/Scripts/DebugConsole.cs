/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:15:36
 * @Description: DebugConsoleFairyGUI 管理类 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace DebugConsoleFairyGUI
{
    public class DebugConsole : MonoBehaviour
    {
        private static DebugConsole instance = null;

        private UIDebugConsole m_mainUI;

        #region 列表数据

        // log 条目列表
        private List<LogEntry> logEntrys;

        // 合并字典
        private Dictionary<LogEntry, int> logCollapsedEntryDic;

        // UI当前显示的列表
        [HideInInspector]
        public List<LogEntry> logShowEntrys;

        #endregion

        #region  

        public bool collapsed;
        public bool infoSelected;
        public bool warnSelected;
        public bool errorSelected;

        #endregion

        void OnEnable()
        {
            Application.logMessageReceived -= ReceivedLog;
            Application.logMessageReceived += ReceivedLog;

            if (instance == null)
            {
                // 添加UI包
                UIPackage.AddPackage(LogConst.UI_PACKAGE_PATH);

                // log 条目列表
                logEntrys = new List<LogEntry>(128);
                logShowEntrys = new List<LogEntry>(128);
                logCollapsedEntryDic = new Dictionary<LogEntry, int>(128);
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= ReceivedLog;
        }

        private void Awake()
        {
            // 设置UI分辨率
            GRoot.inst.SetContentScaleFactor(1136, 640, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
        }

        private void Start()
        {
            // 创建主UI
            m_mainUI = new UIDebugConsole(this);
            m_mainUI.Show();
        }

        private float temp = 0;
        private int index = 0;
        private void Update()
        {
            temp += Time.deltaTime;
            if (temp > 1.0f)
            {
                Debug.Log("日志输出 : " + index);

                temp = 0;
                index++;
            }
        }

        private void ReceivedLog(string logString, string stackTrace, LogType logType)
        {
            LogEntry logEntry = new LogEntry(GetDebugConsoleTag(logString), logString, stackTrace, logType);

            logEntrys.Add(logEntry);

            if (!collapsed)
            {
                if (infoSelected || warnSelected || errorSelected)
                {
                    logShowEntrys.Add(logEntry);
                }
            }
            else
            {
                int index = -1;
                if (logCollapsedEntryDic.TryGetValue(logEntry, out index))
                {
                    logShowEntrys[index].count++;
                }
                else
                {
                    logCollapsedEntryDic.Add(logEntry, logShowEntrys.Count);
                    logShowEntrys.Add(logEntry);
                }
            }

            m_mainUI.Refresh();
        }

        // 获取LogTag
        private string GetDebugConsoleTag(string logContent)
        {
            return "";
        }

        // 清理Log
        public void ClearLog()
        {
            logEntrys.Clear();
            logShowEntrys.Clear();
            logCollapsedEntryDic.Clear();

            m_mainUI.Refresh();
        }
    }
}


