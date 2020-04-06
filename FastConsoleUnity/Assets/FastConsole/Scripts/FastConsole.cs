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
using UnityEditor;
using System.Reflection;

namespace FastConsoleFairyGUI
{
    public class FastConsole : MonoBehaviour
    {
        private static FastConsole _inst;
        public static FastConsole inst
        {
            get
            {
                if (_inst == null)
                {
                    GameObject go = new GameObject();
                    go.name = "FastConsole";
                    _inst = go.AddComponent<FastConsole>();
                }
                return _inst;
            }
        }

        public static Options options;

        public UIMain mainUI { get; private set; }
        public UIDetail detailUI { get; private set; }
        public UISetting settingUI { get; private set; }

        // count
        public int infoCount { get; private set; }
        public int warnCount { get; private set; }
        public int errorCount { get; private set; }

        // selected
        private bool m_infoSelected;
        public bool infoSelected
        {
            get { return m_infoSelected; }
            set
            {
                m_infoSelected = value;
                Recalculate();
                mainUI.Refresh();
            }
        }

        private bool m_warnSelected;
        public bool warnSelected
        {
            get { return m_warnSelected; }
            set
            {
                m_warnSelected = value;
                Recalculate();
                mainUI.Refresh();
            }
        }

        private bool m_errorSelected;
        public bool errorSelected
        {
            get { return m_errorSelected; }
            set
            {
                m_errorSelected = value;
                Recalculate();
                mainUI.Refresh();
            }
        }

        // collapsed
        private bool m_collapsed;
        public bool collapsed
        {
            get { return m_collapsed; }
            set
            {
                m_collapsed = value;
                Recalculate();
                mainUI.Refresh();
            }
        }

        // 搜索字符串
        private string m_searchTarget;
        public string searchTarget
        {
            set
            {
                m_searchTarget = value;
            }
        }

        // 显示日志集合
        public List<LogEntry> viewLogs { get; private set; }
        // 所有日志集合
        private List<LogEntry> m_logs;
        // 合并日志集合
        private Dictionary<int, LogEntry> m_merges;
        // 主线程刷新视图
        private bool m_mainThreadRefreshView;

        // 命令
        private Dictionary<string, Command> m_commandDictionary = new Dictionary<string, Command>();
        private Trie<string> m_commandsTrie = new Trie<string>();

        // systeminfo
        private FPSCounter mFPSCounter = new FPSCounter();
        public FPSCounter fpsCounter { get { return mFPSCounter; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="enabled"></param>
        public void Initialize(bool enabled)
        {
            if (!enabled) return;
            DontDestroyOnLoad(this);
            // log 条目列表
            m_logs = new List<LogEntry>(128);
            viewLogs = new List<LogEntry>(128);
            m_merges = new Dictionary<int, LogEntry>();
            // 默认设置
            m_collapsed = false;
            m_infoSelected = true;
            m_warnSelected = true;
            m_errorSelected = true;
            // command
            CollectCommands();
            // options
            options = Resources.Load("FastConsoleOptions") as Options;
            // UI
            mainUI = new UIMain();
            detailUI = new UIDetail();
            settingUI = new UISetting();
            // 显示主界面
            mainUI.Show();

            // 注册日志监听
            Application.logMessageReceivedThreaded -= ReceivedLog;
            Application.logMessageReceivedThreaded += ReceivedLog;
        }

        void OnDestory()
        {
            Application.logMessageReceived -= ReceivedLog;
        }

        void Update()
        {
            if (_inst == null) return;
            mFPSCounter.OnUpdate();

            if (m_mainThreadRefreshView)
            {
                m_mainThreadRefreshView = false;
                mainUI.Refresh();
            }
        }

        #region log

        private void ReceivedLog(string logString, string stackTrace, LogType logType)
        {
            if (_inst == null) return;
            var logEntry = new LogEntry(logString, stackTrace, logType);
            m_logs.Add(logEntry);
            AddViewEntry(logEntry);
            m_mainThreadRefreshView = true;
        }

        private void AddViewEntry(LogEntry logEntry)
        {
            bool addCount = false;
            if (!collapsed)
            {
                if (MatchFilter(logEntry))
                    viewLogs.Add(logEntry);
                addCount = true;
            }
            else
            {
                int key = logEntry.GetHashCode();
                if (m_merges.ContainsKey(key))
                {
                    m_merges[key].logCount++;
                }
                else
                {
                    if (MatchFilter(logEntry))
                    {
                        m_merges.Add(logEntry.GetHashCode(), logEntry);
                        viewLogs.Add(logEntry);
                        addCount = true;
                    }
                }
            }

            if (addCount)
            {
                if (logEntry.logType == LogType.Log) infoCount++;
                else if (logEntry.logType == LogType.Warning) warnCount++;
                else errorCount++;
            }
        }

        /// <summary>
        /// 匹配过滤字符串
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>是否匹配成功</returns>
        private bool MatchFilter(LogEntry entry)
        {
            if ((infoSelected && entry.logType == LogType.Log)
            || (warnSelected && entry.logType == LogType.Warning)
            || (errorSelected && (entry.logType == LogType.Error || entry.logType == LogType.Assert || entry.logType == LogType.Exception)))
            {
                if (!string.IsNullOrEmpty(m_searchTarget))
                {
                    Regex rgx = new Regex(m_searchTarget);
                    Match match = rgx.Match(entry.logContent);
                    return match.Success;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重新计算数据
        /// </summary>
        private void Recalculate()
        {
            infoCount = 0;
            warnCount = 0;
            errorCount = 0;

            viewLogs.Clear();
            m_merges.Clear();

            for (int i = 0; i < m_logs.Count; i++)
            {
                AddViewEntry(m_logs[i].Reset());
            }
        }

        /// <summary>
        /// 清理日志
        /// </summary>
        public void Cleanup()
        {
            m_logs.Clear();
            Recalculate();
            mainUI.Refresh();
        }

        #endregion

        #region  command
        /// <summary>
        /// 收集所有命令
        /// </summary>
        private void CollectCommands()
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            var types = assembly.GetTypes();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            foreach (var type in types)
            {
                var methods = type.GetMethods(flags);
                foreach (var method in methods)
                {
                    var atr = method.GetCustomAttribute(typeof(MethodCommandAttribute), false);
                    if (atr != null)
                    {
                        if (method.IsStatic) AddStaticMethodCommand(method, atr as MethodCommandAttribute);
                    }
                }
            }
        }

        /// <summary>
        /// 添加静态方法命令
        /// </summary>
        /// <param name="info"></param>
        /// <param name="attr"></param>
        private void AddStaticMethodCommand(MethodInfo info, MethodCommandAttribute attr)
        {
            if (m_commandDictionary.ContainsKey(attr.name))
            {
                Debug.LogError("Multiple commands with the same name are not allowed. [" + attr.name + "]");
                return;
            }
            else
            {
                var command = new StaticMethodCommand(attr.name, attr.description, info);
                m_commandDictionary.Add(attr.name, command);
                m_commandsTrie.Add(new TrieEntry<string>(attr.name, attr.name));
            }
        }

        /// <summary>
        /// 添加静态方法命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="methodName"></param>
        /// <param name="type"></param>
        public void AddStaticMethodCommand(string name, string description, string methodName, Type type)
        {
            if (m_commandDictionary.ContainsKey(name))
            {
                Debug.LogError("Multiple commands with the same name are not allowed. [" + name + "]");
                return;
            }
            else
            {
                var info = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (info != null)
                {
                    var command = new StaticMethodCommand(name, description, info);
                    m_commandDictionary.Add(name, command);
                    m_commandsTrie.Add(new TrieEntry<string>(name, name));
                }
            }
        }

        /// <summary>
        /// 添加方法命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="methodName"></param>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public void AddMethodCommand(string name, string description, string methodName, Type type, object target)
        {
            if (target == null) return;
            if (m_commandDictionary.ContainsKey(name))
            {
                Debug.LogError("Multiple commands with the same name are not allowed. [" + name + "]");
                return;
            }
            else
            {
                var info = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (info != null)
                {
                    var command = new MethodCommand(name, description, info, target);
                    m_commandDictionary.Add(name, command);
                    m_commandsTrie.Add(new TrieEntry<string>(name, name));
                }
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="line"></param>
        public void ExecuteCommand(string line)
        {
            var words = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                Command command = null;
                m_commandDictionary.TryGetValue(words[0], out command);
                if (command != null)
                {
                    command.Execute(line);
                }
            }
        }
        #endregion
    }
}


