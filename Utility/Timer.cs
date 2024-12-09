using AndrewDowsett.CommonObservers;
using System;
using UnityEngine;

namespace AndrewDowsett.Utility
{
    public class Timer : IUpdateObserver
    {
        public Timer(float duration, Action callback, float endingBufferTime = 0f)
        {
            maxDuration = duration;
            this.duration = duration;
            this.callback = callback;
            this.endingBufferTime = endingBufferTime;

            UpdateManager.RegisterObserver(this);
        }

        public bool Running => duration > 0 && !paused;

        private float maxDuration;
        private float duration;
        private Action callback;
        private float endingBufferTime;
        private bool paused;

        public void ObservedUpdate(float deltaTime)
        {
            if (paused)
                return;

            if (duration > 0)
            {
                duration = Mathf.Clamp(duration - deltaTime, 0, duration);
            }

            if (duration == 0 && callback != null)
            {
                if (endingBufferTime >= 0)
                    endingBufferTime -= deltaTime;
                if (endingBufferTime < 0)
                {
                    endingBufferTime = 0;
                    InvokeThenStop();
                }
            }
        }

        public void Pause()
        {
            paused = true;
        }

        public void UnPause()
        {
            paused = false;
        }

        public void Stop()
        {
            duration = 0;
            endingBufferTime = 0;
            callback = null;
        }

        public void InvokeThenStop()
        {
            callback?.Invoke();
            Stop();
        }
    }
}