using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class MapInfo : MonoBehaviour
    {
        public static MapInfo Instance;
        public List<SecurityCamera> informantBaseSecurityCameras = new();
        public Transform informantBaseMapUpper;
        public Transform informantBaseMapLower;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
                Instance = this;
            }
        }
    }
}
