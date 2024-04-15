using FishNet.Object;
using UnityEngine;

namespace Game.Player
{
    public class PlayerSpawnedObject : NetworkBehaviour
    {
        [SerializeField] private GameObject lobbyPlayerPrefab;
        [SerializeField] private GameObject gamePlayerPrefab;
        
        public override void OnStartClient()
        {
            if (base.IsOwner)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyMenu")
                {
                    SpawnLobbyPlayer();
                }
                else
                {
                    SpawnGamePlayer();
                }
            }
        }

        [ServerRpc]
        private void SpawnLobbyPlayer()
        {
            GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab, transform.position, transform.rotation);
            ServerManager.Spawn(lobbyPlayer, base.Owner);
            Destroy(gameObject);
        }
        
        [ServerRpc]
        private void SpawnGamePlayer()
        {
            GameObject gamePlayer = Instantiate(gamePlayerPrefab, transform.position, transform.rotation);
            ServerManager.Spawn(gamePlayer, base.Owner);
            Destroy(gameObject);
        }
    }
}