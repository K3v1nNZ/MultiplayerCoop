using System;
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
        [SerializeField] private GameObject gunObjectViewmodel;
        [SerializeField] private GameObject firstPersonAssets;
        [SerializeField] private GameObject thirdPersonAssets;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool canShoot = true;
        [HideInInspector] public bool canInteract = true;
        [HideInInspector] public bool isRunning;
        private CharacterController _characterController;
        private PlayerInputActions _inputActions;
        private Transform _playerCamera;
        private Vector3 _moveDirection;
        private Vector2 _moveInput;
        private float _rotationY;
        private float _reloadTime;
        private float _fireRateTime;
        private int _currentAmmo;
        private WeaponScriptableObject _previousWeapon;
        public WeaponScriptableObject currentWeapon;
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
                _playerCamera = Camera.main.transform.parent;
                _playerCamera.transform.SetParent(transform);
                _playerCamera.transform.localPosition = new Vector3(0f, 0.725f, 0f);
                firstPersonAssets.SetActive(true);
                thirdPersonAssets.SetActive(false);
                firstPersonAssets.transform.parent = _playerCamera;
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
            Movement();
            EquippedWeapon();
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

        private void EquippedWeapon()
        {
            if (currentWeapon == null) return;
            if (_reloadTime > 0)
            {
                _reloadTime -= Time.deltaTime;
            }
            if (_fireRateTime > 0)
            {
                _fireRateTime -= Time.deltaTime;
            }
            if (_previousWeapon != currentWeapon)
            {
                foreach (Transform child in gunObjectViewmodel.transform)
                {
                    Destroy(child.gameObject);
                }
                GameObject weaponModel = Instantiate(currentWeapon.weaponModel, gunObjectViewmodel.transform);
                _reloadTime = 0;
                _fireRateTime = 0;
                _previousWeapon = currentWeapon;
            }
            if (_fireRateTime > 0 || _reloadTime > 0) return;
            
            switch (currentWeapon.weaponType)
            {
                case WeaponScriptableObject.WeaponType.None:
                    return;
                case WeaponScriptableObject.WeaponType.Ranged:
                    if (currentWeapon.fireMode == WeaponScriptableObject.FireMode.Single && _inputActions.Player.ItemPrimary.WasPressedThisFrame() && _fireRateTime <= 0)
                    {
                        ShootBullet();
                        _fireRateTime = 1 / currentWeapon.fireRate;
                    }
                    else if (currentWeapon.fireMode == WeaponScriptableObject.FireMode.Automatic && _inputActions.Player.ItemPrimary.IsPressed() && _fireRateTime <= 0)
                    {
                        ShootBullet();
                        _fireRateTime = 1 / currentWeapon.fireRate;
                    }
                    break;
                case WeaponScriptableObject.WeaponType.Melee:
                    return;
                default:
                    Debug.Log("Unknown weapon type equipped");
                    return;
            }
        }

        private void ShootBullet()
        {
            RaycastHit hit;
            if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, 100f) && hit.collider.gameObject.TryGetComponent(out IShootable shootableObj))
            {
                shootableObj.Shoot(this);
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

            if (canMove && _playerCamera != null)
            {
                _rotationY += -_inputActions.Player.Look.ReadValue<Vector2>().y * mouseSensitivity;
                _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
                _playerCamera.transform.localRotation = Quaternion.Euler(_rotationY, 0, 0);
                transform.rotation *= Quaternion.Euler(0, _inputActions.Player.Look.ReadValue<Vector2>().x * mouseSensitivity, 0);
            }
        }
    }
}
