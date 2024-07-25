using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
    public class EquipmentScriptableObject : ScriptableObject
    {
        public string equipmentName;
    }
}