using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AndrewDowsett.SceneLoading
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
        public bool CurrentlyLoadingScene => asyncOperation != null || asyncOperationQueue.Count > 0;

        [SerializeField] private MMF_Player startLoadingCanvasFeedbacks;
        [SerializeField] private MMF_Player finishLoadingCanvasFeedbacks;
        [SerializeField] private MMF_Player loadingHintFeedbacks;
        [SerializeField] private TMP_Text loadingHintText;
        [SerializeField] private bool debugging;

        private Scene currentScene;
        private AsyncOperation asyncOperation;
        private List<Action<string, bool, LoadSceneMode>> asyncOperationQueue = new();
        private List<string> asyncOperationSceneNameQueue = new();

        public void Initialize()
        {
            Instance = this;
        }

        public async void LoadScene(string sceneName, bool useLoadingCanvas = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            if (asyncOperation != null)
            {
                DebugMessage($"Loading <{sceneName}> has been added to the queue as there is currently a scene loading/unloading. . .");
                asyncOperationQueue.Add(LoadScene);
                asyncOperationSceneNameQueue.Add(sceneName);
                return;
            }

            if (asyncOperationSceneNameQueue.Count > 0)
            {
                if (sceneName == asyncOperationSceneNameQueue[0])
                {
                    asyncOperationQueue.Remove(asyncOperationQueue.Last());
                    asyncOperationSceneNameQueue.RemoveAt(0);
                }
                else
                {
                    DebugMessage($"LoadScene tried to load a scene not in the queue ({sceneName}). Skipping this load. . .");
                    return;
                }
            }
            DebugMessage($"<{sceneName}> has started loading. . .");

            if (startLoadingCanvasFeedbacks != null && useLoadingCanvas)
            {
                startLoadingCanvasFeedbacks.PlayFeedbacks();
                await UniTask.Delay((int)((startLoadingCanvasFeedbacks.TotalDuration + 0.1f) * 1000));
            }

            asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            await asyncOperation;

            currentScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(currentScene);

            DebugMessage($"<{sceneName}> has been loaded. . .");
            asyncOperation = null;

            if (asyncOperationQueue.Count > 0)
            {
                if (asyncOperationQueue.Last().Method.Name == "LoadScene")
                    LoadScene(asyncOperationSceneNameQueue[0]);
                else
                    UnloadScene(asyncOperationSceneNameQueue[0]);
            }
            else
            {
                if (finishLoadingCanvasFeedbacks != null && useLoadingCanvas)
                {
                    finishLoadingCanvasFeedbacks.PlayFeedbacks();
                    await UniTask.Delay((int)((finishLoadingCanvasFeedbacks.TotalDuration + 0.1f) * 1000));
                }
            }
        }
        public void UnloadScene(string sceneName, bool useLoadingCanvas = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            if (asyncOperation != null)
            {
                DebugMessage($"Unloading <{sceneName}> has been added to the queue as there is currently a scene loading/unloading. . .");
                asyncOperationQueue.Add(UnloadScene);
                asyncOperationSceneNameQueue.Add(sceneName);
                return;
            }

            if (asyncOperationSceneNameQueue.Count > 0)
            {
                bool isLoaded = false;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name == sceneName)
                    {
                        isLoaded = true;
                    }
                }
                if (!isLoaded)
                {
                    asyncOperationQueue.Remove(asyncOperationQueue.Last());
                    asyncOperationSceneNameQueue.RemoveAt(0);
                    DebugMessage($"UnloadScene tried to unload a scene not loaded ({sceneName}). Skipping this unload. . .");
                    return;
                }

                if (sceneName == asyncOperationSceneNameQueue[0])
                {
                    asyncOperationQueue.Remove(asyncOperationQueue.Last());
                    asyncOperationSceneNameQueue.RemoveAt(0);
                }
                else
                {
                    DebugMessage($"UnloadScene tried to unload a scene not in the queue ({sceneName}). Skipping this unload. . .");
                    return;
                }
            }
            else
            {
                DebugMessage($"UnloadScene tried to unload a scene when the queue is empty. Skipping this unload. . .");
                return;
            }

            DebugMessage($"<{sceneName}> has started unloading. . .");

            asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            asyncOperation.completed += (AsyncOperation obj) =>
            {
                DebugMessage($"<{sceneName}> has been unloaded. . .");
                asyncOperation = null;
                if (asyncOperationQueue.Count > 0)
                {
                    if (asyncOperationQueue.Last().Method.Name == "LoadScene")
                        LoadScene(asyncOperationSceneNameQueue[0]);
                    else
                        UnloadScene(asyncOperationSceneNameQueue[0]);
                }
            };
        }

        public void GetRandomHint()
        {
            loadingHintText.text = TextForLoadingScreens.GetRandomLoadingText();
            ShowHintText();
        }
        private void ShowHintText()
        {
            loadingHintText.gameObject.SetActive(true);
        }
        public void HideHintText()
        {
            loadingHintText.gameObject.SetActive(false);
        }

        private void DebugMessage(string message)
        {
            if (debugging)
                Debug.Log(message);
        }
    }
}