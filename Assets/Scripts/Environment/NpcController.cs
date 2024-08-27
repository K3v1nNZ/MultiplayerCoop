using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Environment
{
    public class NpcController : NetworkBehaviour
    {
        [SerializeField] private GameObject hatContainer;
        [SerializeField] private GameObject upperContainer;
        [SerializeField] private GameObject lowerContainer;
        [HideInInspector] public NpcClothingItem hatItem;
        [HideInInspector] public NpcClothingItem upperItem;
        [HideInInspector] public NpcClothingItem lowerItem;
        private NavMeshAgent _navMeshAgent;
        public readonly SyncVar<int> hatClothingItem = new();
        public readonly SyncVar<int> upperClothingItem = new();
        public readonly SyncVar<int> lowerClothingItem = new();
        public readonly SyncVar<bool> target = new();

        [Server]
        public void SetupNpc(int hat, int upper, int lower)
        {
            hatClothingItem.Value = hat;
            upperClothingItem.Value = upper;
            lowerClothingItem.Value = lower;
            
            InstantiateData();
        }
        
        [ObserversRpc(BufferLast = true)]
        private void InstantiateData()
        {
            hatItem = Resources.LoadAll<NpcClothingItem>("NpcClothingItems").ToList().Find(x => x.id == hatClothingItem.Value);
            upperItem = Resources.LoadAll<NpcClothingItem>("NpcClothingItems").ToList().Find(x => x.id == upperClothingItem.Value);
            lowerItem = Resources.LoadAll<NpcClothingItem>("NpcClothingItems").ToList().Find(x => x.id == lowerClothingItem.Value);
            
            Instantiate(hatItem.itemPrefab, hatContainer.transform);
            Instantiate(upperItem.itemPrefab, upperContainer.transform);
            Instantiate(lowerItem.itemPrefab, lowerContainer.transform);
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}