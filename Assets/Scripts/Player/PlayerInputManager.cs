using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;
        public PlayerInputActions PlayerInputActions;
        
        private void Awake()
        {
            if (Instance == null)
            {
                PlayerInputActions = new PlayerInputActions();
                PlayerInputActions.Enable();
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}