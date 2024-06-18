using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Game.Networking
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        [SerializeField] private int maxPlayers;
        public Lobby CurrentLobbyId;
        public bool inLobby;
        private Friend _lobbyOwner;
        public List<Friend> LobbyMembers;
        // Events
        public delegate void LobbyMemberJoin(Lobby lobby, Friend friend);
        public static event LobbyMemberJoin GameLobbyMemberJoin;
        public delegate void LobbyMemberLeave(Lobby lobby, Friend friend);
        public static event LobbyMemberLeave GameLobbyMemberLeave;
        public delegate void LobbySettingsChanged(string message);
        public static event LobbySettingsChanged GameLobbySettingsChanged;

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
            SteamMatchmaking.OnChatMessage += OnLobbyMessage;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnectionState;
        }
        
        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoin;
            SteamMatchmaking.OnLobbyMemberDisconnected -= OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
            SteamMatchmaking.OnChatMessage -= OnLobbyMessage;
            SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequest;
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnectionState;
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log("OnLobbyEntered");
            CurrentLobbyId = lobby;
            _lobbyOwner = lobby.Owner;
            inLobby = true;
            if (CurrentLobbyId.IsOwnedBy(SteamClient.SteamId))
            {
                LobbyMembers = new List<Friend>();
            }
            else
            {
                LobbyMembers = new List<Friend>(lobby.Members);
                if (LobbyMembers.Contains(new Friend(SteamClient.SteamId)))
                {
                    LobbyMembers.Remove(new Friend(SteamClient.SteamId));
                }
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyMenu");
        }

        private void OnLobbyMemberJoin(Lobby lobby, Friend friend)
        {
            if (!lobby.Id.IsValid) return;
            Debug.Log("OnLobbyMemberJoin");
            LobbyMembers.Add(friend);
            GameLobbyMemberJoin?.Invoke(lobby, friend);
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            if (!lobby.Id.IsValid) return;
            Debug.Log("OnLobbyMemberLeave");
            if (friend.Id == _lobbyOwner.Id)
            {
                lobby.Leave();
                inLobby = false;
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyMenu")
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                }
                LobbyMembers.Clear();
                return;
            }
            LobbyMembers.Remove(friend);
            GameLobbyMemberLeave?.Invoke(lobby, friend);
        }
        
        private void OnLobbyMessage(Lobby lobby, Friend friend, string message)
        {
            if (!lobby.Id.IsValid) return;
            Debug.Log("OnLobbyMessage");
            if (message.StartsWith("LOBBY:"))
            {
                if (CurrentLobbyId.IsOwnedBy(friend.Id))
                {
                    GameLobbySettingsChanged?.Invoke(message);
                }
            }
            
        }

        private async void OnGameLobbyJoinRequest(Lobby lobby, SteamId friend)
        {
            if (!lobby.Id.IsValid) return;
            if (lobby.MemberCount >= maxPlayers)
            {
                Debug.LogError("Lobby is full.");
                return;
            }
            if (lobby.GetData("Version") != Application.version)
            {
                Debug.LogError("Version mismatch.");
                Debug.Log(Application.version);
                Debug.Log(lobby.GetData("Version"));
                return;
            }
            LeaveLobby();
            RoomEnter result = await lobby.Join();
            if (result == RoomEnter.Success)
            {
                InstanceFinder.ClientManager.StartConnection(lobby.Owner.Id.ToString());
            }
        }

        private void OnClientConnectionState(NetworkConnection connection, RemoteConnectionStateArgs state)
        {
            if (state.ConnectionState == RemoteConnectionState.Stopped)
            {
                foreach (NetworkObject behaviour in connection.Objects)
                {
                    InstanceFinder.ServerManager.Despawn(behaviour);
                }
                return;
            }
            
            SteamId id = new() { Value = ulong.Parse(connection.GetAddress()) };
            if (state.ConnectionState == RemoteConnectionState.Started && !LobbyMembers.Contains(new Friend(id)) && id != SteamClient.SteamId)
            {
                Debug.Log("Client connection attempted from non-lobby member. Disconnecting client.");
                connection.Disconnect(true);
            }
        }

        public void StartGame()
        {
            if (!CurrentLobbyId.IsOwnedBy(SteamClient.SteamId)) return;
            CurrentLobbyId.SetJoinable(false);
            SceneLoadData data = new(CurrentLobbyId.GetData("Map"));
            data.ReplaceScenes = ReplaceOption.All;
            InstanceFinder.SceneManager.LoadGlobalScenes(data);
        }
        
        public async Task<bool> JoinLobby()
        {
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.FilterDistanceWorldwide().RequestAsync();
            if (lobbies is { Length: > 0 })
            {
                foreach (Lobby lobby in lobbies)
                {
                    if (lobby.Owner.Id == SteamClient.SteamId)
                    {
                        Debug.Log("Cannot join own lobby.");
                        inLobby = false;
                        continue;
                    }

                    if (lobby.GetData("Version") != Application.version)
                    {
                        Debug.Log("Version mismatch.");
                        inLobby = false;
                        continue;
                    }
                    if (lobby.MemberCount < maxPlayers)
                    {
                        RoomEnter result = await lobby.Join();
                        if (result == RoomEnter.Success)
                        {
                            InstanceFinder.ClientManager.StartConnection(lobby.Owner.Id.ToString());
                            return true;
                        }
                        else
                        {
                            Debug.LogError("Failed to join lobby.");
                            inLobby = false;
                            continue;
                        }
                    }
                }
                Debug.LogError("No lobbies found.");
                inLobby = false;
                return false;
            }
            else
            {
                Debug.LogError("No lobbies found.");
                inLobby = false;
                return false;
            }
        }

        public void LeaveLobby()
        {
            if (inLobby)
            {
                CurrentLobbyId.Leave();
                inLobby = false;
                InstanceFinder.ClientManager.StopConnection();
                InstanceFinder.ServerManager.StopConnection(true);
            }
            else
            {
                Debug.Log("Player is not in a lobby.");
            }
        }
        
        public async Task<bool> CreateLobby()
        {
            Lobby? lobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
            if (!lobby.HasValue)
            {
                return false;
            }
            if (InstanceFinder.ServerManager.StartConnection())
            {
                InstanceFinder.ClientManager.StartConnection(SteamClient.SteamId.ToString());
            }
            else
            {
                CurrentLobbyId.Leave();
                inLobby = false;
                return false;
            }
            CurrentLobbyId = lobby.Value;
            _lobbyOwner = CurrentLobbyId.Owner;
            inLobby = true;
            CurrentLobbyId.SetPrivate();
            CurrentLobbyId.SetData("Version", Application.version);
            CurrentLobbyId.SetData("Visibility", "Private");
            return true;
        }
    }
}