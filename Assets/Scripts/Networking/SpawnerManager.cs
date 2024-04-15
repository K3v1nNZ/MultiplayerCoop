using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
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
            _spawnIndex.Value++;
            ServerManager.Spawn(playerObject, player);
        }
    }
}