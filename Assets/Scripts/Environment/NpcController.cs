using System.Collections;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Networking;
using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class NpcController : NetworkBehaviour, IShootable
    {
        [SerializeField] private GameObject hatContainer;
        [SerializeField] private GameObject upperContainer;
        [SerializeField] private GameObject lowerContainer;
        [HideInInspector] public NpcClothingItem hatItem;
        [HideInInspector] public NpcClothingItem upperItem;
        [HideInInspector] public NpcClothingItem lowerItem;
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
        }

        public void Shoot(PlayerController shoot)
        {
            if (target.Value)
            {
                GameEnd(true);
            }
            else
            {
                GameEnd(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void GameEnd(bool win)
        {
            GameEndObserver(win);
        }

        [ObserversRpc(ExcludeOwner = false)]
        private void GameEndObserver(bool win)
        {
            if (win)
            {
                NpcSpawner.Instance.winScreen.SetActive(true);
                StartCoroutine(WaitTillEnd());
            }
            else
            {
                NpcSpawner.Instance.loseScreen.SetActive(true);
                StartCoroutine(WaitTillEnd());
            }
        }

        private IEnumerator WaitTillEnd()
        {
            yield return new WaitForSeconds(5f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}