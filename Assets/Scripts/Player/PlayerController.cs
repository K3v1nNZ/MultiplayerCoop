using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerController : NetworkBehaviour
    {
        public static PlayerController Instance;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float gravity;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private GameObject firstPersonAssets;
        [SerializeField] private GameObject thirdPersonAssets;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool canInteract = true;
        [HideInInspector] public bool isRunning;
        private CharacterController _characterController;
        private PlayerInputActions _inputActions;
        private Camera playerCamera;
        private Vector3 _moveDirection;
        private Vector2 _moveInput;
        private float _rotationY;
        public PlayerRole playerRole;
        public enum PlayerRole
        {
            Assassin,
            Informant,
            Infiltrator,
            Hacker
        }

        public override void OnStartClient()
        {
            if (base.IsOwner)
            {
                Instance = this;
                _inputActions = PlayerInputManager.Instance.PlayerInputActions;
                playerCamera = Camera.main;
                playerCamera.transform.SetParent(transform);
                playerCamera.transform.localPosition = new Vector3(0f, 0.725f, 0f);
                firstPersonAssets.SetActive(true);
                thirdPersonAssets.SetActive(false);
                _characterController = GetComponent<CharacterController>();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                switch (playerRole)
                {
                    case PlayerRole.Assassin:
                        HUDController.Instance.SetRole("Assassin");
                        break;
                    case PlayerRole.Informant:
                        HUDController.Instance.SetRole("Informant");
                        break;
                    case PlayerRole.Infiltrator:
                        HUDController.Instance.SetRole("Infiltrator");
                        break;
                    case PlayerRole.Hacker:
                        HUDController.Instance.SetRole("Hacker");
                        break;
                    default:
                        HUDController.Instance.SetRole("Unknown");
                        break;
                }
            }
            else
            {
                firstPersonAssets.SetActive(false);
                thirdPersonAssets.SetActive(true);
            }
        }

        private void Update()
        {
            if (!base.IsOwner) return;
            Debug.Log(_characterController.isGrounded);
            Movement();
            if (canMove)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Movement()
        {
            _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            isRunning = _inputActions.Player.Sprint.IsPressed();
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float vertical = canMove ? (isRunning ? runSpeed : walkSpeed) * _moveInput.y : 0;
            float horizontal = canMove ? (isRunning ? runSpeed : walkSpeed) * _moveInput.x : 0;
            float moveDirectionY = _moveDirection.y;
            _moveDirection = (forward * vertical) + (right * horizontal);
            _moveDirection = Vector3.ClampMagnitude(_moveDirection, (isRunning ? runSpeed : walkSpeed));
            
            if (_inputActions.Player.Jump.WasPressedThisFrame() && canMove && _characterController.isGrounded)
            {
                _moveDirection.y = jumpForce;
            }
            else
            {
                _moveDirection.y = moveDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= gravity * Time.deltaTime;
            }
            
            if (_characterController.isGrounded && _moveDirection.y < 0)
            {
                _moveDirection.y = -0.5f;
            }
            
            _characterController.Move(_moveDirection * Time.deltaTime);

            if (canMove && playerCamera != null)
            {
                _rotationY += -_inputActions.Player.Look.ReadValue<Vector2>().y * mouseSensitivity;
                _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationY, 0, 0);
                transform.rotation *= Quaternion.Euler(0, _inputActions.Player.Look.ReadValue<Vector2>().x * mouseSensitivity, 0);
            }
        }
    }
}
