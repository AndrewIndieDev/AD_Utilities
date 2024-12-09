using Unity.Netcode;
using UnityEngine;

namespace AndrewDowsett.Networking
{
    public class CustomNetworkManager : NetworkManager
    {
        public static CustomNetworkManager Instance;
        public GameObject customNetworkManagerPrefab;

        public void Start()
        {
            Instance = this;
            SetSingleton();
            DontDestroyOnLoad(gameObject);
        }

        public void ResetServerManager()
        {
            Instantiate(customNetworkManagerPrefab);
            Destroy(gameObject);
        }
    }
}