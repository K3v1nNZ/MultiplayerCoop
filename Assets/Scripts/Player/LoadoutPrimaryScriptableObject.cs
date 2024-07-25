using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Loadout Primary", menuName = "Loadout Primary")]
    public class LoadoutPrimaryScriptableObject : LoadoutItemScriptableObject
    {
        public WeaponScriptableObject weapon;
    }
}