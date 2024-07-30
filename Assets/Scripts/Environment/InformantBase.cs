using UnityEngine;

namespace Game.Environment
{
    public class InformantBase : MonoBehaviour
    {
        public static InformantBase Instance;
        public Transform playerCamHolder;
        public Transform mapCamPos;
        public Transform laptopCamPos;
        public Transform boardCamPos;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}