using FishNet;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

namespace Game.Networking
{
    public class ConnectionManager : MonoBehaviour
    {
        public static ConnectionManager Instance;

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

        private void Start()
        {
            LobbyManager.Instance.CurrentLobbyId.Leave();
            LobbyManager.Instance.CurrentLobbyId = default;
            LobbyManager.Instance.inLobby = false;
            Destroy(LobbyManager.Instance.gameObject);
        }

        private void Update()
        {
            SteamClient.RunCallbacks();
        }

        private void OnEnable()
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }

        private void OnDisable()
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
        }

        private void OnClientConnectionState(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
