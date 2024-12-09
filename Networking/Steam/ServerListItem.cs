using AndrewDowsett.Utility;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace AndrewDowsett.Networking.Steam
{
    public class ServerListItem : MonoBehaviour
    {
        public Lobby steamLobby;
        public Button button;
        public TMP_Text nameText;
        public TMP_Text playerCountText;
        public TMP_Text gameModeText;
        public TMP_Text currentMapText;
        public Image lockImage;

        public new string name;
        public string ip;
        public string gameMode;
        public string currentMap;
        public int port;
        public int currentPlayers;
        public int maxPlayers;
        public bool hasPassword;

        [Header("Password Protected Dialogue")]
        public string passwordProtectedTitleText;
        public string passwordProtectedContentText;

        public void Join()
        {
            if (hasPassword)
            {
                PopupManager.Instance.ShowDialoguePanel(passwordProtectedTitleText, passwordProtectedContentText, EDialoguePanelType.OK, true, (r, s) =>
                {
                    //PlayerPartyManager.Instance.JoinLobby(steamLobby.Id);
                });
            }
            else
            {
                //PlayerPartyManager.Instance.JoinLobby(steamLobby.Id);
            }
        }

        public void SetLobby(Lobby lobby)
        {
            steamLobby = lobby;
            nameText.text = lobby.GetData("LobbyName");
            playerCountText.text = $"{lobby.MemberCount}/{lobby.MaxMembers}";
        }
    }
}