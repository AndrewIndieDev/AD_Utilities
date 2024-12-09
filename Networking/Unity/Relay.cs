using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace AndrewDowsett.Networking.Unity
{
    public class Relay : MonoBehaviour
    {
        public static Relay Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        [Header("Settings")]
        [SerializeField] private bool IsWebBuild;
        [SerializeField] private int maxPlayers;

        [Header("Debug")]
        [SerializeField] private bool debugging;

        /// <summary>
        /// You need to connect to the server before creating or joining a relay server.
        /// </summary>
        private async void Connect()
        {
            try
            {
                DebugMessage("Initializing Unity Services. . .");
                var options = new InitializationOptions();
// used if we are using ParrelSync for clones
//#if UNITY_EDITOR
//                if (ParrelSync.ClonesManager.IsClone())
//                {
//                    string customArgument = ParrelSync.ClonesManager.GetArgument();
//                    options.SetProfile(customArgument);
//                }
//#endif
                await UnityServices.InitializeAsync(options);
                DebugMessage("Completed Successfully. . .");
            }
            catch (System.Exception e)
            {
                DebugMessage($"Failed to initialize Unity Services. . . {e.Message}");
            }

            try
            {
                DebugMessage("Signing in annonymously. . .");
                SignInOptions options = new SignInOptions();
// used if we are using ParrelSync for clones
//#if UNITY_EDITOR
//                if (ParrelSync.ClonesManager.IsClone())
//                {
//                    // When using a ParrelSync clone, switch to a different authentication profile to force the clone
//                    // to sign in as a different anonymous user account.
//                    string customArgument = ParrelSync.ClonesManager.GetArgument();
//                    AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
//                }
//#endif
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                DebugMessage("Signed in successfully. . .");
                DebugMessage($"ID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (System.Exception e)
            {
                DebugMessage($"Failed to sign in. . . {e.Message}");
            }

            NetworkManager.Singleton.OnClientStopped += (isHostServer) =>
            {
                if (!isHostServer)
                {
                    DebugMessage("Server Stopped. . .");
                }
            };

            NetworkManager.Singleton.OnServerStopped += (isHostServer) =>
            {
                DebugMessage("Server Stopped. . .");
            };
        }

        public async void CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                DebugMessage($"Join Code created: {joinCode}. . .");
#if UNITY_WEBGL
                RelayServerData serverData = new RelayServerData(allocation, "wss");
#else
                RelayServerData serverData = new RelayServerData(allocation, "dtls");
#endif

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

                NetworkManager.Singleton.StartServer();

                NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
                {
                    DebugMessage($"Client {clientId} connected. . .");
                };
                NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
                {
                    DebugMessage($"Client {clientId} disconnected. . .");
                };
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                joinCode = joinCode.ToLower();
                DebugMessage($"Joining lobby with joincode: {joinCode}. . .");
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
#if UNITY_WEBGL
                RelayServerData serverData = new RelayServerData(joinAllocation, "wss");
#else
                RelayServerData serverData = new RelayServerData(joinAllocation, "dtls");
#endif
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        private void DebugMessage(string message)
        {
            if (debugging)
                Debug.Log(message);
        }
    }
}