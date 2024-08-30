using UnityEngine;

namespace Game.Environment
{
    [CreateAssetMenu(fileName = "NpcClothingItem", menuName = "Npc Clothing Item")]
    public class NpcClothingItem : ScriptableObject
    {
        public string itemName;
        public string itemDescription;
        public int id;
        public GameObject itemPrefab;
        public ClothingType clothingType;
        public enum ClothingType
        {
            Hat,
            Upper,
            Lower
        }
    }
}