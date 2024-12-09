using AndrewDowsett.Utility;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;

namespace AndrewDowsett.Networking
{
    public class ClientNetworkData : NetworkBehaviour
    {
        #region Variables    
        public NetworkVariable<FixedString32Bytes> nv_PlayerName = new NetworkVariable<FixedString32Bytes>(
            "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
        public NetworkVariable<bool> nv_Ready = new(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        public NetworkVariable<int> nv_PreviousPing = new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        #endregion

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                nv_PlayerName.Value = $"{AuthenticationService.Instance.PlayerName.SanitizeAuthenticationString()}";
            }
        }
    }
}