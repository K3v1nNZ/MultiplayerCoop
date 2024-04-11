using System.Threading.Tasks;
using FishNet;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Networking
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        [SerializeField] private int maxPlayers;
        public Lobby CurrentLobbyId;
        public bool InLobby;
        private Friend _lobbyOwner;
        // Events
        public delegate void LobbyMemberJoin(Lobby lobby, Friend friend);
        public static event LobbyMemberJoin GameLobbyMemberJoin;
        public delegate void LobbyMemberLeave(Lobby lobby, Friend friend);
        public static event LobbyMemberLeave GameLobbyMemberLeave;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            SteamClient.RunCallbacks();
        }

        private void OnEnable()
        {
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoin;
            SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
        }
        
        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoin;
            SteamMatchmaking.OnLobbyMemberDisconnected -= OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log("OnLobbyEntered");
            CurrentLobbyId = lobby;
            _lobbyOwner = lobby.Owner;
            InLobby = true;
            SceneManager.LoadScene("LobbyMenu");
        }

        private void OnLobbyMemberJoin(Lobby lobby, Friend friend)
        {
            Debug.Log("OnLobbyMemberJoin");
            GameLobbyMemberJoin?.Invoke(lobby, friend);
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            Debug.Log("OnLobbyMemberLeave");
            if (friend.Id == _lobbyOwner.Id)
            {
                CurrentLobbyId.Leave();
                InLobby = false;
                if (SceneManager.GetActiveScene().name == "LobbyMenu")
                {
                    SceneManager.LoadScene("MainMenu");
                }
                return;
            }
            GameLobbyMemberLeave?.Invoke(lobby, friend);
            
        }

        private void OnGameLobbyJoinRequest(Lobby lobby, SteamId friend)
        {
            if (lobby.MemberCount >= maxPlayers)
            {
                Debug.LogError("Lobby is full.");
                return;
            }
            LeaveLobby();
            lobby.Join();
        }
        
        public async Task<bool> JoinLobby()
        {
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.FilterDistanceWorldwide().RequestAsync();
            if (lobbies != null && lobbies.Length > 0)
            {
                if (lobbies[0].Owner.Id != SteamClient.SteamId)
                {
                    await lobbies[0].Join();
                }
                else
                {
                    Debug.LogError("Cannot join own lobby.");
                    InLobby = false;
                    return false;
                }
            }
            else
            {
                Debug.LogError("No lobbies found.");
                InLobby = false;
                return false;
            }
            InLobby = false;
            return false;
        }

        public void LeaveLobby()
        {
            if (InLobby)
            {
                CurrentLobbyId.Leave();
                InLobby = false;
            }
            else
            {
                Debug.Log("Player is not in a lobby.");
            }
        }
        
        public async Task<bool> CreateLobby()
        {
            var lobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
            if (!lobby.HasValue)
            {
                return false;
            }
            CurrentLobbyId = lobby.Value;
            _lobbyOwner = CurrentLobbyId.Owner;
            InLobby = true;
            CurrentLobbyId.SetPublic();
            CurrentLobbyId.SetJoinable(true);
            return true;
        }
    }
}