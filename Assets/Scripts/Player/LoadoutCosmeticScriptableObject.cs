using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Loadout Cosmetic", menuName = "Loadout Cosmetic")]
    public class LoadoutCosmeticScriptableObject : LoadoutItemScriptableObject
    {
        public CosmeticScriptableObject cosmetic;
    }
}