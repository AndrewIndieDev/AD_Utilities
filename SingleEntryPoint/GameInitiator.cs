using AndrewDowsett.CommonObservers;
using AndrewDowsett.IDisposables;
using AndrewDowsett.ObjectPooling;
using AndrewDowsett.SceneLoading;
using AndrewDowsett.TimeManagement;
using AndrewDowsett.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AndrewDowsett.SingleEntryPoint
{
    public class GameInitiator : MonoBehaviour
    {
        [Header("Initiator Settings")]
        public EProgressBarType loadingBarType;

        [Header("Scene Post Initiation")]
        public string SceneToLoad;

        [Header("Prefabs to Instantiate")]
        // All of these can be deleted if you don't need them.
        // You can add as many as you want to add here.
        // Remember to also add or remove them in the BindObjects method.
        public EntryScreen _entryScreen;
        public Camera _camera;
        public UpdateManager _updateManager;
        public FixedUpdateManager _fixedUpdateManager;
        public LateUpdateManager _lateUpdateManager;
        public TickSystem _tickSystem;
        public SceneLoader _sceneLoader;
        public IntroAnimation _introAnimation;
        public ObjectPool[] _objectPools;

        private async void Start()
        {
            BindObjects();

            // We can use an IDisposable here to simplify showing and hiding the loading screen:
            using (var loadingSceneDisposable = new DisposableShowEntryScreen(_entryScreen, loadingBarType))
            {
                await InitializeObjects(loadingSceneDisposable, 0.1f);
                await CreateObjects(loadingSceneDisposable, 0.5f);
                await PrepareGame(loadingSceneDisposable, 0.1f);
                await LoadStartingScenes(loadingSceneDisposable, 0.3f);

                loadingSceneDisposable.SetLoadingText("Done...");
                loadingSceneDisposable.SetLoadingBarPercent(1.00f);
            }

            await BeginGame();
        }

        private void BindObjects()
        {
            // Bind all objects
            _entryScreen = Instantiate(_entryScreen);
            _camera = Instantiate(_camera);
            _updateManager = Instantiate(_updateManager);
            _fixedUpdateManager = Instantiate(_fixedUpdateManager);
            _lateUpdateManager = Instantiate(_lateUpdateManager);
            _tickSystem = Instantiate(_tickSystem);
            _sceneLoader = Instantiate(_sceneLoader);
            _introAnimation = Instantiate(_introAnimation);
            for (int i = 0; i < _objectPools.Length; i++)
            {
                _objectPools[i] = Instantiate(_objectPools[i]);
            }
        }

        private async UniTask InitializeObjects(DisposableShowEntryScreen loadingSceneDisposable, float percentageToUse)
        {
            // Perform initialization for analytics/steam etc.
            loadingSceneDisposable.SetLoadingText("Initializing...");

            _tickSystem.Initialize();
            _sceneLoader.Initialize();
            for (int i = 0; i < _objectPools.Length; i++)
            {
                _objectPools[i].Initialize();
            }

            await UniTask.Delay(500);
            loadingSceneDisposable.SetLoadingText("Finished Initializing...");
            loadingSceneDisposable.SetLoadingBarPercent(loadingSceneDisposable.GetLoadingBarPercent() + percentageToUse);
            await UniTask.Delay(500);
        }

        private async UniTask CreateObjects(DisposableShowEntryScreen loadingSceneDisposable, float percentageToUse)
        {
            // Instantiate all objects into the scene
            float initialPercent = loadingSceneDisposable.GetLoadingBarPercent();
            float percentPerPool = percentageToUse / _objectPools.Length;
            for (int i = 0; i < _objectPools.Length; i++)
            {
                loadingSceneDisposable.SetLoadingText("Creating " + _objectPools[i].name + "...");
                await _objectPools[i].SpawnDefaultPool(loadingSceneDisposable, percentPerPool);
            }
            loadingSceneDisposable.SetLoadingBarPercent(initialPercent + percentageToUse);
            loadingSceneDisposable.SetLoadingText("Finished Pooling Objects...");

            await UniTask.Delay(500);
        }

        private async UniTask PrepareGame(DisposableShowEntryScreen loadingSceneDisposable, float percentageToUse)
        {
            // Prepare objects in the scene, if they need methods called before the game starts
            loadingSceneDisposable.SetLoadingText("Preparing Game...");
            await UniTask.Delay(1000);
            loadingSceneDisposable.SetLoadingText("Finished Preparing Game...");
            loadingSceneDisposable.SetLoadingBarPercent(loadingSceneDisposable.GetLoadingBarPercent() + percentageToUse);
            await UniTask.Delay(500);
        }

        private async UniTask LoadStartingScenes(DisposableShowEntryScreen loadingSceneDisposable, float percentageToUse)
        {
            loadingSceneDisposable.SetLoadingText("Loading Scene...");
            // Load the main menu scene
            _sceneLoader.LoadScene(SceneToLoad);
            // Wait for the scenes to finish loading
            await UniTask.WaitUntil(() => !_sceneLoader.CurrentlyLoadingScene);
            loadingSceneDisposable.SetLoadingText("Finished Loading Scene...");
            loadingSceneDisposable.SetLoadingBarPercent(loadingSceneDisposable.GetLoadingBarPercent() + percentageToUse);
            await UniTask.Delay(500);
        }

        private async UniTask BeginGame()
        {
            Destroy(_camera.gameObject);

            // Wait for the intro animation
            // Suggestion: Back a black background in the animation so the player can't see the scene's laoding in.
            await _introAnimation.Play();

            // Unload the entry scene
            _sceneLoader.UnloadScene(gameObject.scene.name);
        }
    }
}