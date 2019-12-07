/*
 * @Author: fasthro
 * @Date: 2019-03-04 16:36:12
 * @Description: Native
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastConsoleFairyGUI
{
    public class FastConsoleNative
    {

        #region android
        // android
        private string m_packageName;
        private string m_nativeClassName;
        private AndroidJavaClass m_nativeClass;
        private AndroidJavaObject m_currentActivity;
        #endregion

        #region ios

#if !UNITY_EDITOR && UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void _copy(string text);
#endif
        #endregion


        // 初始化 Android
        public void InitializeAndroid(string packageName)
        {
            m_packageName = packageName;
            m_nativeClassName = packageName + ".AndroidNative";

#if !UNITY_EDITOR && UNITY_ANDROID
            m_nativeClass = new AndroidJavaClass(m_nativeClassName);
            m_currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
#endif
        }

        public void Release()
        {
            m_nativeClass = null;
            m_currentActivity = null;
        }

        // copy
        public void Copy(string content)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            m_nativeClass.CallStatic("Copy", m_currentActivity, content);
#elif !UNITY_EDITOR && UNITY_IPHONE
            _copy(content);
#else
            GUIUtility.systemCopyBuffer = content;
#endif
        }
    }
}
