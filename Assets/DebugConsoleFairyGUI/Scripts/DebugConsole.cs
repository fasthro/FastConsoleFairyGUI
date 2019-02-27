/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:15:36
 * @Description: DebugConsoleFairyGUI 管理类 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text.RegularExpressions;

namespace DebugConsoleFairyGUI
{
    public enum DebugLogType
    {
        Log,
        Warning,
        Error,
    }

    public class DebugConsole : MonoBehaviour
    {
        private static DebugConsole instance = null;

        // 主界面
        [HideInInspector]
        public UIDebugConsole mainUI;

        // 设置配置
        [HideInInspector]
        public DebugConsoleSetting settingConfig;

        #region 列表数据

        // log 条目列表
        private List<LogEntry> logEntrys;

        // 合并字典<日志条目,在logShowEntrys列表中的索引>
        private Dictionary<LogEntry, int> sameEntryDic;

        // 合并所有条目字典<日志条目,在logShowEntrys列表中的索引>
        private Dictionary<LogEntry, bool> sameEntryAllDic;

        // 过滤字符串
        private string m_filter;

        // UI当前显示的列表
        [HideInInspector]
        public List<LogEntry> logShowEntrys;

        // 日志数量
        [HideInInspector]
        public int infoCount;
        [HideInInspector]
        public int warnCount;
        [HideInInspector]
        public int errorCount;
        #endregion

        #region 状态
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
        private bool m_infoSelected = true;
        public bool infoSelected
        {
            get
            {
                return m_infoSelected;
            }
        }

        // warn
        private bool m_warnSelected = true;
        public bool warnSelected
        {
            get
            {
                return warnSelected;
            }
        }

        // error
        private bool m_errorSelected = true;
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
                logEntrys = new List<LogEntry>(128);
                logShowEntrys = new List<LogEntry>(128);
                sameEntryDic = new Dictionary<LogEntry, int>(128);
                sameEntryAllDic = new Dictionary<LogEntry, bool>(128);
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
            // 初始化配置
            settingConfig = Resources.Load("DebugConsoleSetting") as DebugConsoleSetting;

            // 创建主UI
            mainUI = new UIDebugConsole(this);
            mainUI.Show();
        }

        private float temp = 0;
        private void Update()
        {
            temp += Time.deltaTime;
            if (temp > 0.1f)
            {
                var ran = Random.Range(1, 5);
                if (ran == 1)
                {
                    //Debug.Log("日\n志\n输\n出 : " + ran);
                }
                else if (ran == 3)
                {
                    //Debug.LogWarning("日志输出 : " + ran);
                }
                else
                {
                    //Debug.LogError("日志输出 : " + ran);
                }
                temp = 0;
            }
        }

        private void ReceivedLog(string logString, string stackTrace, LogType logType)
        {
            LogEntry logEntry = new LogEntry(logString, stackTrace, ULTToDLT(logType));

            logEntrys.Add(logEntry);

            SetReceivedLog(logEntry);

            mainUI.Refresh();
        }

        // 设置log
        private void SetReceivedLog(LogEntry logEntry)
        {
            if (!m_collapsed)
            {
                if (FilterRule(logEntry))
                {
                    logShowEntrys.Add(logEntry);
                }

                SetCount(logEntry);
            }
            else
            {
                int index = -1;
                if (sameEntryDic.TryGetValue(logEntry, out index))
                {
                    logShowEntrys[index].logCount++;
                }
                else
                {
                    if (FilterRule(logEntry))
                    {
                        sameEntryDic.Add(logEntry, logShowEntrys.Count);

                        logShowEntrys.Add(logEntry);
                    }
                }

                if (!sameEntryAllDic.ContainsKey(logEntry))
                {
                    sameEntryAllDic.Add(logEntry, true);

                    SetCount(logEntry);
                }
            }
        }

        // Unity LogType to DebugLogType
        private DebugLogType ULTToDLT(LogType logType)
        {
            if (logType == LogType.Log)
                return DebugLogType.Log;
            else if (logType == LogType.Warning)
                return DebugLogType.Warning;
            return DebugLogType.Error;
        }

        // 日志过滤规则
        private bool FilterRule(LogEntry entry)
        {
            if ((m_infoSelected && entry.logType == DebugLogType.Log)
            || (m_warnSelected && entry.logType == DebugLogType.Warning)
            || (m_errorSelected && entry.logType == DebugLogType.Error))
            {
                if (!string.IsNullOrEmpty(m_filter))
                {
                    Regex rgx = new Regex(m_filter);
                    Match match = rgx.Match(entry.logContent);
                    return match.Success;
                }
                return true;
            }
            return false;
        }

        // 设置日志数量
        private void SetCount(LogEntry logEntry)
        {
            if (logEntry.logType == DebugLogType.Log)
            {
                infoCount++;
            }
            else if (logEntry.logType == DebugLogType.Warning)
            {
                warnCount++;
            }
            else if (logEntry.logType == DebugLogType.Error)
            {
                errorCount++;
            }
        }

        // 重置
        private void Reset()
        {
            infoCount = 0;
            warnCount = 0;
            errorCount = 0;

            logShowEntrys.Clear();
            sameEntryDic.Clear();
            sameEntryAllDic.Clear();

            for (int i = 0; i < logEntrys.Count; i++)
            {
                logEntrys[i].Reset();
                SetReceivedLog(logEntrys[i]);
            }

            mainUI.Refresh();
        }

        // 清理Log
        public void ClearLog()
        {
            logEntrys.Clear();
            Reset();
            mainUI.Refresh();
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
            this.m_infoSelected = selected;

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

        // 设置过滤内容
        public void SetFilter(string filter)
        {
            this.m_filter = filter;

            Reset();
        }

        #region 日志输出函数
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        #endregion
    }
}


