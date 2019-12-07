/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:15:36
 * @Description: FastConsole
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text.RegularExpressions;
using System;

namespace FastConsole
{
    /// <summary>
    /// Log Type
    /// </summary>
    public enum ConsoleLogType
    {
        Log,
        Warning,
        Error,
    }

    public class FastConsole : MonoBehaviour
    {
        public static FastConsole inst { get; private set; }

        public FastConsoleNative native { get; private set; }
        public Command command { get; private set; }
        public UIFastConsole mainUI { get; private set; }
        public UIFastConsoleDetail detailUI { get; private set; }
        public UIFastConsoleSetting settingUI { get; private set; }

        // count
        public int infoCount { get; private set; }
        public int warnCount { get; private set; }
        public int errorCount { get; private set; }

        // selected
        public bool infoSelected { get; private set; }
        public bool warnSelected { get; private set; }
        public bool errorSelected { get; private set; }

        // 当前显示的列表
        public List<FastConsoleEntry> viewEntrys { get; private set; }

        // 最小化
        public bool collapsed { get; private set; }

        // log 条目列表
        private List<FastConsoleEntry> m_entrys;
        // 合并字典<日志条目,在viewEntrys列表中的索引>
        private Dictionary<FastConsoleEntry, int> m_sameDic;
        // 合并所有条目字典<日志条目,在viewEntrys列表中的索引>
        private Dictionary<FastConsoleEntry, bool> m_sameAllDic;
        // 过滤字符串
        private string m_filter;

        void Awake()
        {
            Application.logMessageReceived -= ReceivedLog;
            Application.logMessageReceived += ReceivedLog;

            inst = this;
            DontDestroyOnLoad(this);
            // log 条目列表
            m_entrys = new List<FastConsoleEntry>(128);
            viewEntrys = new List<FastConsoleEntry>(128);
            m_sameDic = new Dictionary<FastConsoleEntry, int>(128);
            m_sameAllDic = new Dictionary<FastConsoleEntry, bool>(128);
            // 默认设置
            collapsed = false;
            infoSelected = true;
            warnSelected = true;
            errorSelected = true;
            // native
            native = new FastConsoleNative();
            // 命令
            command = new Command();
            // 添加UI包
            UIPackage.AddPackage("FastConsole/FastConsole");
        }

        void OnDestory()
        {
            Application.logMessageReceived -= ReceivedLog;
            native.Release();
        }

        void Start()
        {
            mainUI = new UIFastConsole();
            detailUI = new UIFastConsoleDetail();
            settingUI = new UIFastConsoleSetting();

            mainUI.Show();
        }

        private void ReceivedLog(string logString, string stackTrace, LogType logType)
        {
            var logEntry = new FastConsoleEntry(logString, stackTrace, ToConsoleLogTyoe(logType));
            m_entrys.Add(logEntry);
            AddEntry(logEntry);
            mainUI.Refresh();
        }

        private void AddEntry(FastConsoleEntry logEntry)
        {
            if (!collapsed)
            {
                if (FilterRule(logEntry))
                    viewEntrys.Add(logEntry);
                SetCount(logEntry);
            }
            else
            {
                int index = -1;
                if (m_sameDic.TryGetValue(logEntry, out index))
                    viewEntrys[index].logCount++;
                else
                {
                    if (FilterRule(logEntry))
                    {
                        m_sameDic.Add(logEntry, viewEntrys.Count);
                        viewEntrys.Add(logEntry);
                    }
                }

                if (!m_sameAllDic.ContainsKey(logEntry))
                {
                    m_sameAllDic.Add(logEntry, true);
                    SetCount(logEntry);
                }
            }
        }

        private ConsoleLogType ToConsoleLogTyoe(LogType logType)
        {
            if (logType == LogType.Log) return ConsoleLogType.Log;
            else if (logType == LogType.Warning) return ConsoleLogType.Warning;
            return ConsoleLogType.Error;
        }

        private bool FilterRule(FastConsoleEntry entry)
        {
            if ((infoSelected && entry.logType == ConsoleLogType.Log)
            || (warnSelected && entry.logType == ConsoleLogType.Warning)
            || (errorSelected && entry.logType == ConsoleLogType.Error))
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


        private void SetCount(FastConsoleEntry logEntry)
        {
            if (logEntry.logType == ConsoleLogType.Log) infoCount++;
            else if (logEntry.logType == ConsoleLogType.Warning) warnCount++;
            else if (logEntry.logType == ConsoleLogType.Error) errorCount++;
        }

        private void Reset()
        {
            infoCount = 0;
            warnCount = 0;
            errorCount = 0;

            viewEntrys.Clear();
            m_sameDic.Clear();
            m_sameAllDic.Clear();

            for (int i = 0; i < m_entrys.Count; i++)
            {
                m_entrys[i].Reset();
                AddEntry(m_entrys[i]);
            }
            mainUI.Refresh();
        }

        public void Clear()
        {
            m_entrys.Clear();
            Reset();
            mainUI.Refresh();
        }

        public void SetCollapsed(bool selected)
        {
            collapsed = selected;
            Reset();
        }

        public void SetInfoSelected(bool selected)
        {
            infoSelected = selected;
            Reset();
        }

        public void SetWarnSelected(bool selected)
        {
            warnSelected = selected;
            Reset();
        }

        public void SetErrorSelected(bool selected)
        {
            errorSelected = selected;
            Reset();
        }

        public void Filter(string filter)
        {
            m_filter = filter;
            Reset();
        }
    }
}


