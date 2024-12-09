using Steamworks;
using Unity.Collections;
using Unity.Netcode;

namespace AndrewDowsett.Networking.Steam
{
    public class SteamClientNetworkData : ClientNetworkData
    {
        #region Variables
        public NetworkVariable<ulong> nv_SteamID = new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

        public ulong SteamID => nv_SteamID.Value;
        public string SteamName => _steamName;

        private string _steamName;
        #endregion

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            nv_PlayerName.OnValueChanged += OnPlayerNameChanged;
            nv_SteamID.OnValueChanged += OnSteamIDChanged;

            if (IsOwner)
            {
                nv_SteamID.Value = SteamClient.SteamId;

                // networking should handle this automatically
                //OnSteamIDChanged(0, nv_SteamID.Value);

                // changing steamID should auto-change the name
                //nv_PlayerName.Value = $"{new Friend(SteamID).Name}";
            }
        }

        public override void OnNetworkDespawn()
        {
            nv_PlayerName.OnValueChanged -= OnPlayerNameChanged;
            nv_SteamID.OnValueChanged -= OnSteamIDChanged;
        }

        private void OnPlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {

        }

        void OnSteamIDChanged(ulong previous, ulong current)
        {
            _steamName = new Friend(current).Name;
            gameObject.name = $"[Client] {_steamName}";
        }
    }
}