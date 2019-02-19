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
        // 收缩
        private bool m_collapsed;
        public bool collapsed
        {
            get
            {
                return m_collapsed;
            }
        }

        //info
        private bool m_infoSelected;
        public bool infoSelected
        {
            get
            {
                return m_infoSelected;
            }
        }

        // warn
        private bool m_warnSelected;
        public bool warnSelected
        {
            get
            {
                return warnSelected;
            }
        }

        // error
        private bool m_errorSelected;
        public bool errorSelected
        {
            get
            {
                return errorSelected;
            }
        }

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
                logEntrys = new List<LogEntry>();
                logShowEntrys = new List<LogEntry>();
                logCollapsedEntryDic = new Dictionary<LogEntry, int>();
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
            LogEntry logEntry = new LogEntry(GetTag(logString), logString, stackTrace, logType);

            logEntrys.Add(logEntry);

            if (!m_collapsed)
            {
                if (GetSelectedRule(logEntry))
                {
                    logShowEntrys.Add(logEntry);
                }
            }
            else
            {
                if (GetSelectedRule(logEntry))
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
            }

            m_mainUI.Refresh();
        }

        // 获取LogTag
        private string GetTag(string logContent)
        {
            return "";
        }

        // 获取选择的规则
        private bool GetSelectedRule(LogEntry entry)
        {
            // var info = infoSelected && entry.logType == LogType.Log;
            // var warn = warnSelected && entry.logType == LogType.Warning;
            // var error = errorSelected && entry.logType == LogType.Error;

            // return info || warn || error;
            Debug.Log(infoSelected);
            Debug.Log(entry.logType);
            Debug.Log(infoSelected && entry.logType == LogType.Log);
            return true;
        }

        // 重新设置选择
        private void Reset()
        {
            logShowEntrys.Clear();
            logCollapsedEntryDic.Clear();

            if (!m_collapsed)
            {
                for (int i = 0; i < logEntrys.Count; i++)
                {
                    if (GetSelectedRule(logEntrys[i]))
                    {
                        logShowEntrys.Add(logEntrys[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < logEntrys.Count; i++)
                {
                    if (GetSelectedRule(logEntrys[i]))
                    {

                    }
                }
            }
        }

        // 清理Log
        public void ClearLog()
        {
            logEntrys.Clear();
            logShowEntrys.Clear();
            logCollapsedEntryDic.Clear();

            m_mainUI.Refresh();
        }

        // 设置收缩
        public void SetCollapsed(bool selected)
        {
            this.m_collapsed = selected;

            Reset();
        }

        // 设置Info
        public void SetInfo(bool selected)
        {
            this.m_infoSelected = m_collapsed;

            Reset();
        }

        // 设置warn
        public void SetWarn(bool selected)
        {
            this.m_warnSelected = selected;

            Reset();
        }

        // 设置error
        public void SetError(bool selected)
        {
            this.m_errorSelected = selected;

            Reset();
        }
    }
}


