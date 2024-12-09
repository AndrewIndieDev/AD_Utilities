using AndrewDowsett.IDisposables;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AndrewDowsett.ObjectPooling
{
    public class ObjectPool : MonoBehaviour
    {
        public static Dictionary<string, ObjectPool> Pools;

        [Header("Only use Custom Pool Name if you want to\ncreate multiple pools of the same object,\notherwise it will use the name of\nthe prefab as the Pool Name.")]
        [SerializeField] private string customPoolName;
        [SerializeField] private GameObject poolPrefab;
        [SerializeField] private int defaultPoolSize;
        [SerializeField] private int maxPoolSize;
        [SerializeField] private bool needsCanvas;
        [SerializeField] private Transform canvas;

        public RectTransform CanvasRect => needsCanvas ? canvas.GetComponent<RectTransform>() : null;

        private ObjectPool<IPooledObject> _pool;
        private bool _isInitialized;
        private Transform parent;

        public void Initialize()
        {
            parent = needsCanvas ? Instantiate(canvas, transform).transform : transform;

            IPooledObject pooledObject = poolPrefab.GetComponent<IPooledObject>();
            if (pooledObject == null)
            {
                Debug.LogError($"{poolPrefab.name} on {gameObject.name} does not implement IPooledObject!");
                return;
            }
            if (Pools == null)
            {
                Pools = new();
            }
            if (customPoolName == string.Empty)
            {
                customPoolName = poolPrefab.name;
            }
            if (Pools.ContainsKey(customPoolName))
            {
                Debug.LogError($"Pool with name {customPoolName} already exists! This pool will not be spawned.");
                return;
            }
            Pools.Add(customPoolName, this);
            _isInitialized = true;
        }

        public async UniTask SpawnDefaultPool(DisposableShowEntryScreen loadingSceneDisposable, float percentToUse)
        {
            if (!_isInitialized)
                return;

            _pool = new ObjectPool<IPooledObject>(
                () => Instantiate(poolPrefab, parent).GetComponent<IPooledObject>(),
                obj => obj.Spawn(this),
                obj => obj.Despawn(),
                null,
                false,
                defaultPoolSize,
                maxPoolSize);

            float initialPercentage = loadingSceneDisposable.GetLoadingBarPercent();

            loadingSceneDisposable.SetLoadingText($"Pooling {defaultPoolSize} {poolPrefab.name}'s...");

            List<IPooledObject> objects = new();
            for (int i = 0; i < defaultPoolSize; i++)
            {
                if (loadingSceneDisposable != null && percentToUse > 0)
                    loadingSceneDisposable.SetLoadingBarPercent(initialPercentage + (percentToUse / defaultPoolSize * (i + 1) * 0.5f));

                objects.Add(_pool.Get());

                if (i % 10 == 1)
                    await UniTask.Yield();
            }

            loadingSceneDisposable.SetLoadingText($"Releasing {defaultPoolSize} {poolPrefab.name}'s...");

            for (int i = 0; i < objects.Count; i++)
            {
                if (loadingSceneDisposable != null && percentToUse > 0)
                    loadingSceneDisposable.SetLoadingBarPercent(initialPercentage + (percentToUse * 0.5f) + (percentToUse / defaultPoolSize * (i + 1) * 0.5f));

                _pool.Release(objects[i]);

                if (i % 10 == 1)
                    await UniTask.Yield();
            }
        }

        public IPooledObject Get() => _pool.Get();

        public void Release(IPooledObject obj) => _pool.Release(obj);
    }
}