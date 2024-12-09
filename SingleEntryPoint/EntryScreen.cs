using AndrewDowsett.CommonObservers;
using AndrewDowsett.Utility;
using UnityEngine;

namespace AndrewDowsett.SingleEntryPoint
{
    public class EntryScreen : MonoBehaviour, IUpdateObserver
    {
        public SerializableDictionary<EProgressBarType, ProgressBar> ProgressBar;

        private float _progress = 0;
        private ProgressBar barToUse;

        public void Show(EProgressBarType eProgressBarType)
        {
            barToUse = ProgressBar[eProgressBarType];
            barToUse.gameObject.SetActive(true);
            UpdateManager.RegisterObserver(this);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            UpdateManager.UnregisterObserver(this);
            gameObject.SetActive(false);
            barToUse.progress.fillAmount = 0;
            barToUse.text.text = string.Empty;
            barToUse.gameObject.SetActive(false);
        }

        public float GetBarPercent()
        {
            return _progress;
        }

        public void SetBarPercent(float percent)
        {
            _progress = percent;
        }

        public void SetBarText(string text)
        {
            barToUse.text.text = text;
        }

        public void ObservedUpdate(float deltaTime)
        {
            if (_progress > barToUse.progress.fillAmount)
            {
                barToUse.progress.fillAmount += deltaTime;
            }

            if (_progress < barToUse.progress.fillAmount)
            {
                barToUse.progress.fillAmount = _progress;
            }
        }
    }
}