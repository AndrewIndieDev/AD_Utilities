using UnityEngine;
using Steamworks;

namespace AndrewDowsett.Networking.Steam
{
    public class SteamLobbyList : MonoBehaviour
    {
        public GameObject serverListItemTemplate;
        public GameObject serverListParent;

        private void Start()
        {
            serverListItemTemplate.SetActive(false);
            RefreshList();
        }

        public async void RefreshList()
        {
            ClearList();

            var lobbies = await SteamMatchmaking.LobbyList
               .FilterDistanceWorldwide()
               .WithMaxResults(100)
               .WithKeyValue("LobbyType", "Game")
               .RequestAsync();

            if (lobbies != null)
            {
                foreach (var lobby in lobbies)
                {
                    GameObject lobbyItem = Instantiate(serverListItemTemplate, serverListParent.transform);
                    //lobbyItem.GetComponent<ServerListItem>().SetLobby(lobby);
                    lobbyItem.SetActive(true);
                }
            }
        }

        public void ClearList()
        {
            for (int i = serverListParent.transform.childCount - 1; i > 0; --i)
            {
                Destroy(serverListParent.transform.GetChild(i).gameObject);
            }
        }
    }
}