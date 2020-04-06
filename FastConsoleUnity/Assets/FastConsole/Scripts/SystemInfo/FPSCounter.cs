/*
 * @Author: fasthro
 * @Date: 2020-04-06 17:51:55
 * @Description: fps
 */

using UnityEngine;

namespace FastConsoleFairyGUI
{
    public class FPSCounter
    {
        // 更新频率
        public float frequency = 0.5f;
        // fps精度(保留小数位数)
        public int mDecimal = 1;

        private float mAccum;
        private int mFrames;
        private float mAccumTime;

        private float mFPSValue;
        public float fpsValue { get { return mFPSValue; } }

        private string mFPS;
        public string fps { get { return mFPS; } }

        public void OnUpdate()
        {
            mAccumTime += Time.deltaTime;
            mAccum += Time.timeScale / Time.deltaTime;
            mFrames++;
            if (mAccumTime >= frequency)
            {
                mFPSValue = mAccum / (float)mFrames;
                mFPS = mFPSValue.ToString("f" + Mathf.Clamp(mDecimal, 0, 10));
                mAccumTime = 0f;
                mAccum = 0f;
                mFrames = 0;
            }
        }
    }
}
