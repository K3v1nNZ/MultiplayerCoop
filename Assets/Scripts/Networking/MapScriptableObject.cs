using UnityEngine;

namespace Game.Networking
{
    [CreateAssetMenu(fileName = "MapScriptableObject", menuName = "MapScriptableObject")]
    public class MapScriptableObject : ScriptableObject
    {
        public string mapName;
        public Object scene;
    }
}
