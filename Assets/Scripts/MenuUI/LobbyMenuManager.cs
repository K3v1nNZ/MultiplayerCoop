using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Networking;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.MenuUI
{
    public class LobbyMenuManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private CanvasGroup lobbyInfoCanvasGroup;
        [SerializeField] private Transform[] playerStands;
        [SerializeField] private GameObject[] playerStandsNameCanvas;
        [SerializeField] private TMP_Text[] playerStandsNameText;
        [SerializeField] private GameObject playerStandPrefab;

        private void Awake()
        {
            fadeCanvasGroup.alpha = 1;
            lobbyInfoCanvasGroup.alpha = 1;
        }

        private void Start()
        {
            fadeCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Linear);
            RefreshPlayerStands();
        }
        
        private void OnEnable()
        {
            LobbyManager.GameLobbyMemberJoin += OnLobbyMemberJoin;
            LobbyManager.GameLobbyMemberLeave += OnLobbyMemberLeave;
        }
        
        private void OnDisable()
        {
            LobbyManager.GameLobbyMemberJoin -= OnLobbyMemberJoin;
            LobbyManager.GameLobbyMemberLeave -= OnLobbyMemberLeave;
        }

        private void RefreshPlayerStands()
        {
            IEnumerable<Friend> lobbyMembers = LobbyManager.Instance.CurrentLobbyId.Members.ToArray();
            
            for (int i = 1; i < lobbyMembers.Count(); i++)
            {
                if (lobbyMembers.ElementAt(i).Id == SteamClient.SteamId)
                {
                    lobbyMembers = lobbyMembers.Take(i).Concat(lobbyMembers.Skip(i + 1));
                    break;
                }
                
                playerStandsNameCanvas[i].SetActive(true);
                playerStandsNameText[i].text = lobbyMembers.ElementAt(i).Name;
                Instantiate(playerStandPrefab, playerStands[i]);
            }
        }

        #region Events

        private void OnLobbyMemberJoin(Lobby lobby, Friend friend)
        {
            RefreshPlayerStands();
        }
        
        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            RefreshPlayerStands();
        }

        #endregion

        #region LobbyInfoButtons

        public void LeaveButton()
        {
            LobbyManager.Instance.LeaveLobby();
            lobbyInfoCanvasGroup.interactable = false;
            lobbyInfoCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() => SceneManager.LoadScene("MainMenu"));
        }

        #endregion
    }
}
