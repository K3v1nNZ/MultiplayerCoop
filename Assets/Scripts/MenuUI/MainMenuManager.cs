using System;
using System.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private CanvasGroup menuButtonsCanvasGroup;
        [SerializeField] private CanvasGroup hostJoinCanvasGroup;
        [SerializeField] private TMP_Text profileName;
        [SerializeField] private RawImage profilePicture;
        
        private void Awake()
        {
            fadeCanvasGroup.alpha = 1;
            menuCanvasGroup.alpha = 1;
            menuButtonsCanvasGroup.alpha = 1;
            hostJoinCanvasGroup.alpha = 0;
        }

        private void Start()
        {
            fadeCanvasGroup.DOFade(0f, 1f).SetEase(Ease.Linear);
            profileName.text = SteamClient.Name;
            GetProfilePicture();
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

            var avatar = new Texture2D((int)image.Value.Width, (int)image.Value.Height, TextureFormat.ARGB32, false);
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

        #region MainMenuButtons
        public void PlayButton()
        {
            menuButtonsCanvasGroup.interactable = false;
            menuButtonsCanvasGroup.blocksRaycasts = false;
            menuButtonsCanvasGroup.DOFade(0f, 0.15f).SetEase(Ease.Linear).OnComplete(() => hostJoinCanvasGroup.interactable = true);
            hostJoinCanvasGroup.DOFade(1f, 0.15f).SetEase(Ease.Linear).OnComplete(() => hostJoinCanvasGroup.blocksRaycasts = true);
        }
        
        public void ShopButton()
        {
            // TODO: Shop menu
            return;
        }

        public void OptionsButton()
        {
            // TODO: Options menu
            return;
        }

        public void QuitButton()
        {
            Application.Quit();
        }
        #endregion
        
        #region HostJoinButtons
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
            hostJoinCanvasGroup.interactable = false;
            hostJoinCanvasGroup.blocksRaycasts = false;
            hostJoinCanvasGroup.DOFade(0f, 0.15f).SetEase(Ease.Linear).OnComplete(() => menuButtonsCanvasGroup.interactable = true);
            menuButtonsCanvasGroup.DOFade(1f, 0.15f).SetEase(Ease.Linear).OnComplete(() => menuButtonsCanvasGroup.blocksRaycasts = true);
        }
        #endregion
    }
}