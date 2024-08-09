using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Game.Networking
{
    public class NpcSpawner : NetworkBehaviour
    {
        public static NpcSpawner Instance;
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private List<Transform> npcSpawnPoints;
        [SerializeField] private List<Transform> targetNpcSpawnPoints;
        

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

        public override void OnStartServer()
        {
            if (IsServer)
            {
                SpawnNpcs();
            }
        }

        private void SpawnNpcs()
        {
            for (int i = 0; i < npcSpawnPoints.Count; i++)
            {
                GameObject npc = Instantiate(npcPrefab, npcSpawnPoints[i].position, Quaternion.identity);
                ServerManager.Spawn(npc);
            }
        }
    }
}
