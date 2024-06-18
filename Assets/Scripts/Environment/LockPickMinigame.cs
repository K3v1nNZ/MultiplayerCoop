using System.Collections.Generic;
using DG.Tweening;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputManager = Game.Player.PlayerInputManager;

namespace Game.Environment
{
    public class LockPickMinigame : MonoBehaviour
    {
        [Range(1, 10)]
        public int pins = 1;
        [Range(0.1f, 2)]
        public float pinSpeed = 1;
        public GameObject pinPrefab;
        public GameObject pinsContainer;
        public Door door;
        private List<LockPickPin> _pins = new();
        private int _activePin;
        private PlayerInputActions _inputActions;
        
        private void Start()
        {
            _inputActions = PlayerInputManager.Instance.PlayerInputActions;
            PlayerController.Instance.canMove = false;
            PlayerController.Instance.canInteract = false;
            for (int i = 0; i < pins; i++)
            {
                GameObject pin = Instantiate(pinPrefab, transform);
                pin.transform.SetParent(pinsContainer.transform);
                LockPickPin lockPickPin = pin.GetComponent<LockPickPin>();
                lockPickPin.speed = pinSpeed;
                _pins.Add(lockPickPin);
            }

            _pins[0].activated = true;
            _activePin = 0;
        }

        private void Update()
        {
            if (_inputActions.UI.Cancel.WasPressedThisFrame())
            {
                PlayerController.Instance.canMove = true;
                PlayerController.Instance.canInteract = true;
                _pins[_activePin].indicator.DOKill();
                Destroy(gameObject);
            }
            
            if (_inputActions.UI.LockPick.WasPressedThisFrame())
            {
                _pins[_activePin].indicator.DOKill();
                PinCheck();
            }

            if (!door.IsLocked.Value)
            {
                PlayerController.Instance.canMove = true;
                PlayerController.Instance.canInteract = true;
                Destroy(gameObject);
            }
        }

        private void PinCheck()
        {
            if (_pins[_activePin].indicator.localPosition.y < _pins[_activePin].height / 2 && _pins[_activePin].indicator.localPosition.y > -_pins[_activePin].height / 2)
            {
                _pins[_activePin].activated = false;
                _activePin++;
                if (_activePin < pins)
                {
                    _pins[_activePin].activated = true;
                }
                else
                {
                    door.UnlockDoor();
                    PlayerController.Instance.canMove = true;
                    PlayerController.Instance.canInteract = true;
                    Destroy(gameObject);
                }
            }
            else
            {
                PlayerController.Instance.canMove = true;
                PlayerController.Instance.canInteract = true;
                Destroy(gameObject);
            }
        }
    }
}
