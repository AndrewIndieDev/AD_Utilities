using System;
using Unity.Netcode;

namespace AndrewDowsett.Networking
{
    public class RPCManager : NetworkBehaviour
    {
        public static RPCManager Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region ServerRPCs
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void PingServerRPC()
        {
            PingClientRPC();
        }
        #endregion

        #region ClientRPCs
        [Rpc(SendTo.ClientsAndHost)]
        void PingClientRPC()
        {
            if (IsOwner)
                PingFromClientServerRPC();
        }
        [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
        void PingFromClientServerRPC()
        {
            DateTime time = DateTime.UtcNow;
            ServerNetworkData.LocalPlayerData.nv_PreviousPing.Value = (int)(time - ServerNetworkData.Instance.LastPingTime).TotalMilliseconds;
        }
        #endregion
    }
}