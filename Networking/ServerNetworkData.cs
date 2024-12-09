using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using AndrewDowsett.Utility;
using System.Collections;
using System;

namespace AndrewDowsett.Networking
{
    public enum EGameState
    {
        Connecting,
        In_Menu,
        In_Lobby,
        In_Game
    }

    public class ServerNetworkData : NetworkBehaviour
    {
        #region Game State
        private NetworkVariable<int> nv_GameUIState = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public void ChangeGameUIState(EGameState state) => nv_GameUIState.Value = (int)state;
        public EGameState GetGameState() => (EGameState)nv_GameUIState.Value;
        public void Sub_GameUIState(NetworkVariable<int>.OnValueChangedDelegate action) => nv_GameUIState.OnValueChanged += action;
        public void UnSub_GameUIState(NetworkVariable<int>.OnValueChangedDelegate action) => nv_GameUIState.OnValueChanged -= action;

        #endregion

        public static ServerNetworkData Instance { get; set; }

        [SerializeField] private ClientNetworkData clientNetworkDataPrefab;

        private DateTime _lastPingTime;

        public static List<ClientNetworkData> AllInstances { get; private set; } = new List<ClientNetworkData>();
        public static ClientNetworkData GetPlayerData(ulong clientID) => AllInstances.Find(x => x.OwnerClientId == clientID);
        public static ClientNetworkData LocalPlayerData => AllInstances.Find(x => x.OwnerClientId == NetworkManager.Singleton.LocalClientId);
        public static int GetPlayerCount() => AllInstances.Count;
        public DateTime LastPingTime => _lastPingTime;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                StartCoroutine(UpdatePing());
            }
        }

        public void OnPlayerConnect(ulong clientId)
        {
            ClientNetworkData clientNetworkData = Instantiate(clientNetworkDataPrefab);
            clientNetworkData.NetworkObject.SpawnAsPlayerObject(clientId, true);

            AllInstances.AddUnique(clientNetworkData);
        }

        public void OnPlayerDisconnect(ulong clientId)
        {
            AllInstances.Remove(GetPlayerData(clientId));
        }

        IEnumerator UpdatePing()
        {
            while (true)
            {
                _lastPingTime = DateTime.UtcNow;
                RPCManager.Instance.PingServerRPC();
                yield return new WaitForSeconds(5.0f);
            }
        }
    }
}