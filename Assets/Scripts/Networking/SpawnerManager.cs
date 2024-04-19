using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Player;
using UnityEngine;

namespace Game.Networking
{
    public class SpawnerManager : NetworkBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject playerPrefab;
        private readonly SyncVar<int> _spawnIndex = new(0);

        public override void OnStartClient()
        {
            SpawnPlayerServerRpc(base.LocalConnection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(NetworkConnection player)
        {
            GameObject playerObject = Instantiate(playerPrefab, spawnPoints[_spawnIndex.Value].position, Quaternion.identity);
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            playerController.playerRole = _spawnIndex.Value switch
            {
                0 => PlayerController.PlayerRole.Assassin,
                1 => PlayerController.PlayerRole.Informant,
                2 => PlayerController.PlayerRole.Infiltrator,
                3 => PlayerController.PlayerRole.Hacker,
                _ => playerController.playerRole
            };
            _spawnIndex.Value++;
            ServerManager.Spawn(playerObject, player);
        }
    }
}