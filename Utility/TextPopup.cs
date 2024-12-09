using TMPro;
using UnityEngine;

namespace AndrewDowsett.Utility
{
    public class TextPopup : MonoBehaviour
    {
        // [SerializeField] private MMF_Player createFeedbacks;

        public static TextPopup Create(Vector3 position, string text, Color color)
        {
            Transform textPopupTransform = Instantiate(Resources.Load<GameObject>("TextPopup"), position, Quaternion.identity).transform;
            TextPopup textPopup = textPopupTransform.GetComponent<TextPopup>();
            textPopup.Setup(text, color);

            return null;
        }

        private TextMeshPro textMesh;

        private void Start()
        {
            textMesh = transform.GetComponent<TextMeshPro>();
        }

        public void Setup(string text, Color color = default)
        {
            textMesh.SetText(text);
            textMesh.color = color;

            // createFeedbacks?.PlayFeedbacks();
        }
    }
}