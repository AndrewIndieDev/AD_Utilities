using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AndrewDowsett.Utility
{
    public class UIMouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Graphic targetGraphic;

        public UnityEvent onPointerEnter;
        public UnityEvent onPointerExit;
        public UnityEvent onPrimaryPointerDown;
        public UnityEvent onPrimaryPointerUp;
        public UnityEvent onSecondaryPointerDown;
        public UnityEvent onSecondaryPointerUp;

        public Color startingColor;
        public Color hoverColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetGraphic != null)
                targetGraphic.color = hoverColor;
            onPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (targetGraphic != null)
                targetGraphic.color = startingColor;
            onPointerExit?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                onPrimaryPointerDown?.Invoke();
            else
                onSecondaryPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                onPrimaryPointerUp?.Invoke();
            else
                onSecondaryPointerUp?.Invoke();
        }
    }
}