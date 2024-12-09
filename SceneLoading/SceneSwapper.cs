using UnityEngine;
using UnityEngine.SceneManagement;

namespace AndrewDowsett.SceneLoading
{
    public class SceneSwapper : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private bool useLoadingCanvas;
        [SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        public void LoadScene()
        {
            SceneLoader.Instance.LoadScene(sceneName, useLoadingCanvas, loadSceneMode);
        }
    }
}