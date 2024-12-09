using System.Collections.Generic;
using UnityEngine;

namespace AndrewDowsett.CommonObservers
{
    public class FixedUpdateManager : MonoBehaviour
    {
        private static List<IFixedUpdateObserver> _observers = new();
        private static List<IFixedUpdateObserver> _pendingObservers = new();
        private static int _currentIndex;

        private void Update()
        {
            for (_currentIndex = _observers.Count - 1; _currentIndex >= 0; _currentIndex--)
            {
                _observers[_currentIndex].ObservedFixedUpdate(Time.fixedDeltaTime);
            }

            _observers.AddRange(_pendingObservers);
            _pendingObservers.Clear();
        }

        public static void RegisterObserver(IFixedUpdateObserver observer)
        {
            _pendingObservers.Add(observer);
        }

        public static void UnregisterObserver(IFixedUpdateObserver observer)
        {
            _observers.Remove(observer);
            _currentIndex--;
        }
    }
}