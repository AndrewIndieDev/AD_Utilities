using System.Collections.Generic;
using UnityEngine;

namespace AndrewDowsett.CommonObservers
{
    public class UpdateManager : MonoBehaviour
    {
        private static List<IUpdateObserver> _observers = new();
        private static List<IUpdateObserver> _pendingObservers = new();
        private static int _currentIndex;

        private void Update()
        {
            for (_currentIndex = _observers.Count - 1; _currentIndex >= 0; _currentIndex--)
            {
                _observers[_currentIndex].ObservedUpdate(Time.deltaTime);
            }

            _observers.AddRange(_pendingObservers);
            _pendingObservers.Clear();
        }

        public static void RegisterObserver(IUpdateObserver observer)
        {
            _pendingObservers.Add(observer);
        }

        public static void UnregisterObserver(IUpdateObserver observer)
        {
            _observers.Remove(observer);
            _currentIndex--;
        }
    }
}