using Game.Networking;
using UnityEngine;

namespace Game.MenuUI.MenuPanels
{
    public class PlayMenuPanel : MenuPanel
    {
        public async void HostButton()
        {
            await LobbyManager.Instance.CreateLobby();
        }

        public async void JoinButton()
        {
            await LobbyManager.Instance.JoinLobby();
        }

        public void BackButton()
        {
            MainMenuManager.Instance.playMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.mainMenuPanel.ShowCanvasGroup();
        }
    }
}
