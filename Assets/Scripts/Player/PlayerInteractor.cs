using UnityEngine;

namespace Game.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRange;
        [SerializeField] private GameObject interactPrompt;
        
        private void Update()
        {
            Ray ray = new(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange) && PlayerController.Instance.canInteract)
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactableObj))
                {
                    interactPrompt.SetActive(true);
                    if (PlayerInputManager.Instance.PlayerInputActions.Player.Interact.WasPressedThisFrame())
                    {
                        interactableObj.Interact(PlayerController.Instance);
                    }
                }
                else
                {
                    interactPrompt.SetActive(false);
                }
            }
            else
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
