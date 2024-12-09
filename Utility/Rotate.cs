using UnityEngine;

namespace AndrewDowsett.Utility
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class Rotate : MonoBehaviour
    {
        public Vector3 rotationAxis;
        public float speed;
        public Vector2 randomRotationOffset;
        public Vector2 randomSpeedOffset;

        private void Start()
        {
            if (!Application.isPlaying) return;
            transform.Rotate(rotationAxis, Random.Range(randomRotationOffset.x, randomRotationOffset.y));
            speed += Random.Range(randomSpeedOffset.x, randomSpeedOffset.y);
        }

        private void Update()
        {
            transform.Rotate(rotationAxis, speed * Time.deltaTime);
        }
    }
}