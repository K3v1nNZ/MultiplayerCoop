using UnityEngine;

namespace Game.Player
{
    public class LoadoutItemScriptableObject : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public string itemDescription;
        public Texture2D itemIcon;
        public ItemUsableRole usableRole;
        public ItemType itemType;
        public enum ItemUsableRole
        {
            All = 0,
            Assassin = 1,
            Infiltrator = 2,
            Hacker = 3
        }
        public enum ItemType
        {
            PrimaryWeapon = 1,
            Equipment = 2,
            Cosmetic = 3
        }
    }
}
