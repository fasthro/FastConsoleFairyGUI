/*
 * @Author: fasthro
 * @Date: 2019-02-18 15:30:41
 * @Description: 日志数据条目类
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugConsoleFairyGUI
{
    public class LogEntry : IEquatable<LogEntry>
    {
        // 日志tag
        public string logTag;
		// 日志类型
		public DebugLogType logType;
        // 日志内容
        public string logContent;
        // 日志堆栈内容
        public string stackContent;
        // 当前日志数量
        public int logCount;
        // 是否选中
        public bool selected;

        // 哈希值
        private int m_hasValue;
        // 是否已经生成了哈希值
        private bool m_generateHasValue;

        // tostring content
        private string m_content;

        public LogEntry(string logTag, string logContent, string stackContent, DebugLogType logType)
        {
            this.logTag = logTag;
            this.logContent = logContent;
            this.stackContent = stackContent;
            this.logType = logType;
            this.logCount = 1;
            this.selected = false;
        }

        public void Reset()
        {
            logCount = 1;
            selected = false;
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(m_content))
                m_content = string.Concat(logContent, "\n\n", stackContent);
            return m_content;
        }

        // 自定义 Equals
        public bool Equals(LogEntry entry)
        {
            return logContent == entry.logContent && stackContent == entry.stackContent;
        }

        // override object.GetHashCode
		// 最快获取字符串哈希方法 https://stackoverflow.com/a/19250516/2373034
        public override int GetHashCode()
        {
            if (!m_generateHasValue)
            {
                unchecked
                {
                    m_hasValue = 17;
                    m_hasValue = m_hasValue * 23 + logContent == null ? 0 : logContent.GetHashCode();
                    m_hasValue = m_hasValue * 23 + stackContent == null ? 0 : stackContent.GetHashCode();
                }
            }
            return m_hasValue;
        }

    }
}
