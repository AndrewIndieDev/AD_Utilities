using UnityEngine;
using UnityEngine.Events;

namespace AndrewDowsett.Utility
{
    public class ObjectMouseEvents : MonoBehaviour
    {
        public MeshRenderer targetMeshRenderer;

        public UnityEvent onPointerEnter;
        public UnityEvent onPointerExit;
        public UnityEvent onPrimaryPointerDown;
        public UnityEvent onPrimaryPointerUp;
        public UnityEvent onSecondaryPointerDown;
        public UnityEvent onSecondaryPointerUp;

        public Color startingColor;
        public Color hoverColor;

        public void OnMouseOver()
        {
            if (targetMeshRenderer != null)
                targetMeshRenderer.sharedMaterial.color = hoverColor;
            onPointerEnter?.Invoke();
        }

        public void OnMouseExit()
        {
            if (targetMeshRenderer != null)
                targetMeshRenderer.sharedMaterial.color = startingColor;
            onPointerExit?.Invoke();
        }

        public void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
                onPrimaryPointerDown?.Invoke();
            else
                onSecondaryPointerDown?.Invoke();
        }

        public void OnMouseUp()
        {
            if (Input.GetMouseButtonDown(0))
                onPrimaryPointerUp?.Invoke();
            else
                onSecondaryPointerUp?.Invoke();
        }
    }
}