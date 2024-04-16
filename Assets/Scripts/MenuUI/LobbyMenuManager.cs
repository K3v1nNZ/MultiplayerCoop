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
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform[] cameraPoints;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private CanvasGroup lobbyInfoCanvasGroup;
        [SerializeField] private GameObject[] lobbySettings;
        [SerializeField] private TMP_Dropdown mapDropdown;
        [SerializeField] private TMP_Text lobbyVisibilityText;
        [SerializeField] private TMP_Text lobbyMapText;
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
            if (LobbyManager.Instance.CurrentLobbyId.IsOwnedBy(SteamClient.SteamId))
            {
                lobbySettings[0].SetActive(true);
                lobbySettings[1].SetActive(false);
            }
            else
            {
                lobbySettings[0].SetActive(false);
                lobbySettings[1].SetActive(true);
            }
            RefreshPlayerStands();
            AddMapsToDropdown();
            SetLobbyInfo();
        }
        
        private void OnEnable()
        {
            LobbyManager.GameLobbyMemberJoin += OnLobbyMemberJoin;
            LobbyManager.GameLobbyMemberLeave += OnLobbyMemberLeave;
            LobbyManager.GameLobbySettingsChanged += GameLobbySettingChanged;
        }
        
        private void OnDisable()
        {
            LobbyManager.GameLobbyMemberJoin -= OnLobbyMemberJoin;
            LobbyManager.GameLobbyMemberLeave -= OnLobbyMemberLeave;
            LobbyManager.GameLobbySettingsChanged -= GameLobbySettingChanged;
        }

        private void RefreshPlayerStands()
        {
            foreach (Transform playerStand in playerStands)
            {
                foreach (Transform child in playerStand)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (GameObject playerStandNameCanvas in playerStandsNameCanvas)
            {
                playerStandNameCanvas.SetActive(false);
            }
            
            for (int i = 0; i < LobbyManager.Instance.LobbyMembers.Count(); i++)
            {
                Instantiate(playerStandPrefab, playerStands[i]);
                playerStandsNameCanvas[i].SetActive(true);
                playerStandsNameText[i].text = LobbyManager.Instance.LobbyMembers.ElementAt(i).Name;
            }
            mainCamera.transform.DOMove(cameraPoints[LobbyManager.Instance.LobbyMembers.Count()].position, 0.5f).SetEase(Ease.OutExpo);
            mainCamera.transform.DORotate(cameraPoints[LobbyManager.Instance.LobbyMembers.Count()].rotation.eulerAngles, 0.5f).SetEase(Ease.OutExpo);
        }

        private void AddMapsToDropdown()
        {
            mapDropdown.options.Clear();
            MapScriptableObject maps = Resources.Load<MapScriptableObject>("Maps/MapList");
            foreach (string map in maps.maps)
            {
                mapDropdown.options.Add(new TMP_Dropdown.OptionData(map));
            }
            LobbyManager.Instance.CurrentLobbyId.SetData("Map", mapDropdown.options[0].text);
        }

        private void SetLobbyInfo()
        {
            if (!LobbyManager.Instance.CurrentLobbyId.IsOwnedBy(SteamClient.SteamId))
            {
                lobbyVisibilityText.text = LobbyManager.Instance.CurrentLobbyId.GetData("Visibility");
                lobbyMapText.text = LobbyManager.Instance.CurrentLobbyId.GetData("Map");
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

        private void GameLobbySettingChanged(string message)
        {
            if (message.Contains("VISIBILITY"))
            {
                lobbyVisibilityText.text = message.Split(':')[2];
            }
            else if (message.Contains("MAP"))
            {
                lobbyMapText.text = message.Split(':')[2];
            }
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
        
        public void StartButton()
        {
            if (LobbyManager.Instance.CurrentLobbyId.IsOwnedBy(SteamClient.SteamId))
            {
                LobbyManager.Instance.StartGame();
            }
        }

        public void InviteButton()
        {
            SteamFriends.OpenGameInviteOverlay(LobbyManager.Instance.CurrentLobbyId.Id);
        }

        public void LobbyVisibility(TMP_Dropdown dropdown)
        {
            if (!LobbyManager.Instance.CurrentLobbyId.IsOwnedBy(SteamClient.SteamId)) return;
            switch (dropdown.value)
            {
                case 0:
                    LobbyManager.Instance.CurrentLobbyId.SetPrivate();
                    LobbyManager.Instance.CurrentLobbyId.SetData("Visibility", "Private");
                    LobbyManager.Instance.CurrentLobbyId.SendChatString("LOBBY:VISIBILITY:Private");
                    Debug.Log("Lobby set to private.");
                    break;
                case 1:
                    LobbyManager.Instance.CurrentLobbyId.SetFriendsOnly();
                    LobbyManager.Instance.CurrentLobbyId.SetData("Visibility", "Friends");
                    LobbyManager.Instance.CurrentLobbyId.SendChatString("LOBBY:VISIBILITY:Friends");
                    Debug.Log("Lobby set to friends only.");
                    break;
                case 2:
                    LobbyManager.Instance.CurrentLobbyId.SetPublic();
                    LobbyManager.Instance.CurrentLobbyId.SetData("Visibility", "Public");
                    LobbyManager.Instance.CurrentLobbyId.SendChatString("LOBBY:VISIBILITY:Public");
                    Debug.Log("Lobby set to public.");
                    break;
            }
        }

        public void MapSelected(TMP_Dropdown dropdown)
        {
            if (!LobbyManager.Instance.CurrentLobbyId.IsOwnedBy(SteamClient.SteamId)) return;
            LobbyManager.Instance.CurrentLobbyId.SetData("Map", dropdown.options[dropdown.value].text);
            LobbyManager.Instance.CurrentLobbyId.SendChatString($"LOBBY:MAP:{dropdown.options[dropdown.value].text}");
        }

        #endregion
    }
}
