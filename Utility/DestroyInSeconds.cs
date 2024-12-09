using AndrewDowsett.CommonObservers;
using UnityEngine;

namespace AndrewDowsett.Utility
{
    public class DestroyInSeconds : MonoBehaviour, IUpdateObserver
    {
        [SerializeField] private float seconds;

        private void Start()
        {
            UpdateManager.RegisterObserver(this);
        }

        public void ObservedUpdate(float deltaTime)
        {
            seconds -= deltaTime;
            if (seconds <= 0)
                Destroy(gameObject);
        }
    }
}