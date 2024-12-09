using AndrewDowsett.SingleEntryPoint;
using System;

namespace AndrewDowsett.IDisposables
{
    public class DisposableShowEntryScreen : IDisposable
    {
        private readonly EntryScreen _loadingScreen;

        public DisposableShowEntryScreen(EntryScreen loadingScreen, EProgressBarType type = EProgressBarType.Bar_Left_To_Right)
        {
            _loadingScreen = loadingScreen;
            _loadingScreen.Show(type);
        }

        public float GetLoadingBarPercent()
        {
            return _loadingScreen.GetBarPercent();
        }

        public void SetLoadingBarPercent(float percent)
        {
            _loadingScreen.SetBarPercent(percent);
        }

        public void SetLoadingText(string text)
        {
            _loadingScreen.SetBarText(text);
        }

        public void Dispose()
        {
            _loadingScreen.Hide();
        }
    }
}