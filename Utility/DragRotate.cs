using UnityEngine;

namespace AndrewDowsett.Utility
{
    public class DragRotate : MonoBehaviour
    {
        public float dragSpeed = 1.0f;

        float prevDragPos;
        private float lastDelta;
        bool dragging = false;

        void Update()
        {
            if (!dragging && lastDelta != 0)
            {
                lastDelta = lastDelta * (1.0f - Time.deltaTime);
                OnMouseDrag();
            }
        }

        private void OnMouseDrag()
        {
            if (dragging)
                lastDelta = (Input.mousePosition.x - prevDragPos);
            transform.Rotate(Vector3.up, -lastDelta / Screen.width * dragSpeed);
            prevDragPos = Input.mousePosition.x;
        }

        private void OnMouseDown()
        {
            prevDragPos = Input.mousePosition.x;
            dragging = true;
        }

        private void OnMouseUp()
        {
            dragging = false;
        }
    }
}