using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class CubeOfWisdom : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerController interactor)
        {
            Debug.Log("You will be hit by a car in approximately 30 seconds.");
        }
    }
}