using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.MenuUI.MenuPanels;
using Game.Networking;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = Steamworks.Data.Color;
using Image = Steamworks.Data.Image;

namespace Game.MenuUI
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;
        public MainMenuPanel mainMenuPanel;
        public PlayMenuPanel playMenuPanel;
        public LobbyBrowserMenuPanel lobbyBrowserMenuPanel;
        public OptionsMenuPanel optionsMenuPanel;
        public LoadoutMenuPanel loadoutMenuPanel;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private TMP_Text profileName;
        [SerializeField] private RawImage profilePicture;
        [SerializeField] private TMP_Text clockText;
        private List<MenuPanel> _previousMenuPanels;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            fadeCanvasGroup.alpha = 1;
        }

        private void Start()
        {
            fadeCanvasGroup.DOFade(0f, 1f).SetEase(Ease.Linear);
            profileName.text = SteamClient.Name;
            GetProfilePicture();
        }

        private void Update()
        {
            clockText.text = DateTime.Now.ToString("HH:mm");
        }

        private async void GetProfilePicture()
        {
            Image? image;
            try
            {
                image = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            Texture2D avatar = new((int)image.Value.Width, (int)image.Value.Height, TextureFormat.ARGB32, false);
            avatar.filterMode = FilterMode.Trilinear;
            for (int x = 0; x < image.Value.Width; x++)
            {
                for (int y = 0; y < image.Value.Height; y++)
                {
                    Color p = image.Value.GetPixel(x, y);
                    avatar.SetPixel(x, (int)image.Value.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
                }
            }
            avatar.Apply();
            profilePicture.texture = avatar;
        }
        
        public void ShowModal(string header, string body, string confirmText, string cancelText, string alternateMessage, Action confirmAction, Action cancelAction = null, Action alternateAction = null)
        {
            GameObject modalPanelInstance = Instantiate(modalPanel, mainCanvas.transform);
            ModalPanel modalPanelScript = modalPanelInstance.GetComponent<ModalPanel>();
            modalPanelScript.ShowModal(header, body, confirmText, cancelText, alternateMessage, confirmAction, cancelAction, alternateAction);
        }

        #region BottomBarButtons
        public void OptionsButton()
        {
            mainMenuPanel.HideCanvasGroup();
            optionsMenuPanel.ShowCanvasGroup();
        }

        public void QuitButton()
        {
            ShowModal("Quit Game", "Are you sure you want to quit?", "Yes", "No", null, Application.Quit);
        }
        #endregion
    }
}