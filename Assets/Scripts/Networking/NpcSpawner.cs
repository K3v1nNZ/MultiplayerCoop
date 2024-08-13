using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Game.Environment;
using UnityEngine;

namespace Game.Networking
{
    public class NpcSpawner : NetworkBehaviour
    {
        public static NpcSpawner Instance;
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private List<Transform> npcSpawnPoints;
        [SerializeField] private List<Transform> targetNpcSpawnPoints;
        private List<NpcClothingItem> _npcClothingItems = new();
        private List<NpcClothingItem> _npcHatItems = new();
        private List<NpcClothingItem> _npcUpperItems = new();
        private List<NpcClothingItem> _npcLowerItems = new();
        [HideInInspector] public bool startSpawning;
        

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
            _npcClothingItems = Resources.LoadAll<NpcClothingItem>("NpcClothingItems").ToList();
            _npcHatItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Hat);
            _npcUpperItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Upper);
            _npcLowerItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Lower);
        }
        
        public override void OnStartServer()
        {
            _npcClothingItems = Resources.LoadAll<NpcClothingItem>("NpcClothingItems").ToList();
            _npcHatItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Hat);
            _npcUpperItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Upper);
            _npcLowerItems = _npcClothingItems.FindAll(x => x.clothingType == NpcClothingItem.ClothingType.Lower);
            
            if (base.IsServerInitialized)
            {
                SpawnNpcs();
            }
        }

        private void SpawnNpcs()
        {
            for (int i = 0; i < npcSpawnPoints.Count; i++)
            {
                GameObject npc = Instantiate(npcPrefab, npcSpawnPoints[i].position, Quaternion.identity);
                NpcController npcController = npc.GetComponent<NpcController>();
                ServerManager.Spawn(npc);
                npcController.SetupNpc(_npcHatItems[Random.Range(0, _npcHatItems.Count)].id, _npcUpperItems[Random.Range(0, _npcUpperItems.Count)].id, _npcLowerItems[Random.Range(0, _npcLowerItems.Count)].id);
            }
            
            GameObject targetNpc = Instantiate(npcPrefab, targetNpcSpawnPoints[Random.Range(0, targetNpcSpawnPoints.Count)].position, Quaternion.identity);
            NpcController targetNpcController = targetNpc.GetComponent<NpcController>();
            ServerManager.Spawn(targetNpc);
            targetNpcController.target.Value = true;
            targetNpcController.SetupNpc(_npcHatItems[Random.Range(0, _npcHatItems.Count)].id, _npcUpperItems[Random.Range(0, _npcUpperItems.Count)].id, _npcLowerItems[Random.Range(0, _npcLowerItems.Count)].id);
        }
    }
}
