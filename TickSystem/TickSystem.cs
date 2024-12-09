using AndrewDowsett.CommonObservers;
using UnityEngine;

namespace AndrewDowsett.TimeManagement
{
    public class TickSystem : MonoBehaviour, IUpdateObserver
    {
        public delegate void Tick();
        public static event Tick OnTick;
        public static int TicksPerSecond;
        public static int CurrentTick;
        public static float TickTime => 1f / TicksPerSecond;

        [SerializeField] private int _ticksPerSecond = 60;
        private int _currentTick;
        private float _deltaTime;

        public void Initialize()
        {
            CurrentTick = 0;
            UpdateManager.RegisterObserver(this);
        }

        public void ObservedUpdate(float deltaTime)
        {
            TicksPerSecond = _ticksPerSecond;
            CurrentTick = _currentTick;

            _deltaTime += deltaTime;
            if (_deltaTime >= TickTime)
            {
                CurrentTick++;
                OnTick?.Invoke();
                _deltaTime -= TickTime;
            }
        }
    }
}