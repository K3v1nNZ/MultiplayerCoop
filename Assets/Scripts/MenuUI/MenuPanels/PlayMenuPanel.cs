using Game.Networking;
using UnityEngine;

namespace Game.MenuUI.MenuPanels
{
    public class PlayMenuPanel : MenuPanel
    {
        public async void HostButton()
        {
            CanvasGroupInteractable(false);
            if (!await LobbyManager.Instance.CreateLobby())
            {
                MainMenuManager.Instance.ShowModal("Error", "Failed to create lobby.", "Continue", null, null, null);
                CanvasGroupInteractable(true);
            }
        }

        public void JoinButton()
        {
            MainMenuManager.Instance.playMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.lobbyBrowserMenuPanel.ShowCanvasGroup();
        }

        public void BackButton()
        {
            MainMenuManager.Instance.playMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.mainMenuPanel.ShowCanvasGroup();
        }
    }
}
