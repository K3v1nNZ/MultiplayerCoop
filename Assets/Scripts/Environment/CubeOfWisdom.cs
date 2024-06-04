using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class CubeOfWisdom : MonoBehaviour, IInteractable, IShootable
    {
        public void Interact(PlayerController interactor)
        {
            Debug.Log("You will be hit by a car in approximately 30 seconds.");
        }
        
        public void Shoot(PlayerController shooter)
        {
            Debug.Log("You have been hit by a car.");
        }
    }
}