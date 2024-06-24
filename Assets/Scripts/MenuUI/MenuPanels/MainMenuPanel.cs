using UnityEngine;

namespace Game.MenuUI.MenuPanels
{
    public class MainMenuPanel : MenuPanel
    {
        public void PlayButton()
        {
            MainMenuManager.Instance.mainMenuPanel.HideCanvasGroup();
            MainMenuManager.Instance.playMenuPanel.ShowCanvasGroup();
        }
        
        public void ShopButton()
        {
            // TODO: Shop menu
            return;
        }

        public void LoadoutButton()
        {
            // TODO: Loadout menu
            return;
        }
    }
}
