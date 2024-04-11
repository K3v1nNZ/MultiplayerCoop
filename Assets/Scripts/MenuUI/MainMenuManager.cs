using DG.Tweening;
using Game.Networking;
using UnityEngine;

namespace Game.MenuUI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private CanvasGroup menuButtonsCanvasGroup;
        [SerializeField] private CanvasGroup hostJoinCanvasGroup;
        
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