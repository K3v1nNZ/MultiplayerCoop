using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Game.MenuUI.MenuPanels
{
    public class LobbyBrowserMenuPanel : MenuPanel
    {
        private bool _previousShownState;
        [SerializeField] private GameObject lobbyListContent;
        [SerializeField] private GameObject lobbyListPrefab;
        
        private async void Update()
        {
            if (_previousShownState != isPanelShown)
            {
                _previousShownState = isPanelShown;
                if (isPanelShown)
                {
                    await RefreshList();
                }
            }
        }

        private async Task RefreshList()
        {
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.FilterDistanceWorldwide().WithSlotsAvailable(1).RequestAsync();
            foreach (Transform child in lobbyListContent.transform)
            {
                Destroy(child.gameObject);
            }

            if (lobbies == null || lobbies.Length == 0)
            {
                MainMenuManager.Instance.ShowModal("Error", "No lobbies found.", "Continue", null, null, null);
                return;
            }
            foreach (Lobby lobby in lobbies)
            {
                GameObject lobbyList = Instantiate(lobbyListPrefab, lobbyListContent.transform);
                lobbyList.GetComponent<LobbyPrefabScript>().SetLobby(lobby);
            }
        }

        public async void RefreshButton()
        {
            await RefreshList();
        }

        public void BackButton()
        {
            MainMenuManager.Instance.lobbyBrowserMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.playMenuPanel.ShowCanvasGroup();
        }
    }
}
