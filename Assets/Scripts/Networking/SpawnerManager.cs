using System.Linq;
using DG.Tweening;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Player;
using UnityEngine;

namespace Game.Networking
{
    public class SpawnerManager : NetworkBehaviour
    {
        public static SpawnerManager Instance;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private CanvasGroup waitingForPlayersCanvas;
        public readonly SyncDictionary<NetworkConnection, PlayerController> _playersConnected = new();
        private bool _spawnedAll;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public override void OnStartClient()
        {
            WaitForSpawn(base.LocalConnection);
        }

        [Server(Logging = LoggingType.Off)]
        private void Update()
        {
            if (!_spawnedAll && _playersConnected.Count == ServerManager.Clients.Count)
            {
                for (int i = 0; i < _playersConnected.Count; i++)
                {
                    GameObject playerObject = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
                    PlayerController playerController = playerObject.GetComponent<PlayerController>();
                    ServerManager.Spawn(playerObject, _playersConnected.Keys.ToArray()[i]);
                    _playersConnected[_playersConnected.Keys.ToArray()[i]] = playerController;
                }
                _spawnedAll = true;
                NpcSpawner.Instance.startSpawning = true;
                AllPlayersLoaded();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void WaitForSpawn(NetworkConnection player)
        {
            _playersConnected.Add(player, null);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void GiveRole(NetworkConnection player)
        {
            PlayerController playerController = _playersConnected[player];
            playerController.SetRoleData(player, (PlayerController.PlayerRole) _playersConnected.Keys.ToList().IndexOf(player));
        }

        [ObserversRpc]
        private void AllPlayersLoaded()
        {
            waitingForPlayersCanvas.DOFade(0f, 0.2f).OnComplete(() => waitingForPlayersCanvas.gameObject.SetActive(false));
        }
    }
}