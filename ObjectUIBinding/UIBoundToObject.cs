using AndrewDowsett.CommonObservers;
using AndrewDowsett.ObjectPooling;
using UnityEngine;
using UnityEngine.UI;

namespace AndrewDowsett.ObjectUIBinding
{
    public class UIBoundToObject : MonoBehaviour, IPooledObject, IUpdateObserver
    {
        private ObjectPool pool;

        [SerializeField] private Image[] images;
        [SerializeField] private float maxDistance = 500f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 offset;

        private ObjectBoundToUI boundTo;
        private Camera mainCam;
        private RectTransform canvasRectTransform;

        public void Spawn(ObjectPool pool)
        {
            this.pool = pool;
            gameObject.SetActive(false);
        }

        public void Despawn()
        {
            mainCam = null;
            boundTo = null;
            gameObject.SetActive(false);
            UpdateManager.UnregisterObserver(this);
        }

        public void ObservedUpdate(float deltaTime)
        {
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp(1.2f - (Vector2.Distance(Input.mousePosition, transform.position) / maxDistance), 0.3f, 1f));
            }

            if (boundTo != null && mainCam != null)
            {
                Vector3 screenPosition = mainCam.WorldToScreenPoint(boundTo.transform.position);
                if (screenPosition != transform.position)
                    transform.position = screenPosition + (Vector3)offset;
            }
            else
            {
                Debug.Log($"{gameObject.name} is either not bound to anything or mainCam is null, returning to the pool.");
                pool.Release(this);
            }
        }

        public void OnClick()
        {
            // Do something
            Debug.Log($"Clicked {gameObject.name}. . .");
        }

        public void Bind(ObjectBoundToUI boundTo)
        {
            this.boundTo = boundTo;
            mainCam = Camera.main;
            canvasRectTransform = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            gameObject.SetActive(true);
            UpdateManager.RegisterObserver(this);
        }
    }
}