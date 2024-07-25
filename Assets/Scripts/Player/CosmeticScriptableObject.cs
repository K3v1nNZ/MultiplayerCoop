using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Cosmetic", menuName = "Cosmetic")]
    public class CosmeticScriptableObject : ScriptableObject
    {
        public string cosmeticName;
        public CosmeticType cosmeticType;
        public GameObject cosmeticModel;
        public enum CosmeticType
        {
            Hat,
            Body
        }
    }
}