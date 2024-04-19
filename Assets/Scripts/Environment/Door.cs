using DG.Tweening;
using FishNet.Object;
using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class Door : NetworkBehaviour, IInteractable
    {
        [SerializeField] private Transform doorParent;
        [SerializeField] private float parentOpenRotation;
        [SerializeField] private float parentCloseRotation;
        private float _openRotation;
        private bool _isOpen;
        private AudioSource _audioSource;
        
        public void Interact(PlayerController interactor)
        {
            ServerToggleDoor(interactor);
        }

        public override void OnStartClient()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ServerToggleDoor(PlayerController interactor)
        {
            if (interactor.transform.position.x > transform.position.x)
            {
                _openRotation = parentOpenRotation;
            }
            else
            {
                _openRotation = -parentOpenRotation;
            }

            if (!_isOpen)
            {
                doorParent.DOLocalRotate(new Vector3(doorParent.localEulerAngles.x, _openRotation, doorParent.localEulerAngles.z), 0.5f).SetEase(Ease.Linear);
                PlayDoorSound();
                _isOpen = true;
            }
            else
            {
                doorParent.DOLocalRotate(new Vector3(doorParent.localEulerAngles.x, parentCloseRotation, doorParent.localEulerAngles.z), 0.5f).SetEase(Ease.Linear);
                PlayDoorSound();
                _isOpen = false;
            }
        }

        [ObserversRpc]
        private void PlayDoorSound()
        {
            _audioSource.Play();
        }
    }
}
