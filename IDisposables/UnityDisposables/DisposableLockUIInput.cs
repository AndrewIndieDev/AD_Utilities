using System;
using UnityEngine.UI;

namespace AndrewDowsett.IDisposables
{
    public class DisposableLockUIInput : IDisposable
    {
        private readonly Graphic _graphic;

        public DisposableLockUIInput(Graphic graphic)
        {
            _graphic = graphic;
            graphic.raycastTarget = false;
        }

        public void Dispose()
        {
            _graphic.raycastTarget = true;
        }
    }
}
