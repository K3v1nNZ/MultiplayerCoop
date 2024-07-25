using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Loadout Equipment", menuName = "Loadout Equipment")]
    public class LoadoutEquipmentScriptableObject : LoadoutItemScriptableObject
    {
        public EquipmentScriptableObject equipment;
    }
}