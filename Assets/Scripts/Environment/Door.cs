using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Player;
using UnityEngine;

namespace Game.Environment
{
    public class Door : NetworkBehaviour, IInteractable
    {
        [SerializeField] private Transform doorParent;
        [SerializeField] private float parentOpenRotation;
        [SerializeField] private float parentCloseRotation;
        [SerializeField] private int pins;
        [SerializeField] private float pinSpeed;
        [SerializeField] private bool startLocked;
        [SerializeField] private bool usePlayerZ;
        public readonly SyncVar<bool> IsLocked = new(false);
        public GameObject lockPickMiniGame;
        private float _openRotation;
        private bool _isOpen;
        private AudioSource _audioSource;
        
        public void Interact(PlayerController interactor)
        {
            if (!IsLocked.Value)
            {
                ServerToggleDoor(interactor);
            }
            else
            {
                GameObject lockPickGame = lockPickMiniGame;
                LockPickMinigame lockPickMinigame = lockPickGame.GetComponent<LockPickMinigame>();
                lockPickMinigame.door = this;
                lockPickMinigame.pins = pins;
                lockPickMinigame.pinSpeed = pinSpeed;
                Instantiate(lockPickMinigame);
            }
        }
        
        public override void OnStartServer()
        {
            IsLocked.Value = startLocked;
        }

        public override void OnStartClient()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void UnlockDoor()
        {
            IsLocked.Value = false;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ServerToggleDoor(PlayerController interactor)
        {
            if (usePlayerZ)
            {
                if (interactor.transform.position.z > transform.position.z)
                {
                    _openRotation = parentOpenRotation;
                }
                else
                {
                    _openRotation = -parentOpenRotation;
                }
            }
            else if (interactor.transform.position.x > transform.position.x)
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
