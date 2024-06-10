using DG.Tweening;
using FishNet.Object;
using FishNet.Transporting;
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
        [SerializeField] private GameObject thirdPersonGunObject;
        [SerializeField] private GameObject firstPersonAssets;
        [SerializeField] private GameObject thirdPersonAssets;
        [SerializeField] private GameObject bulletTrailObject;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool canShoot = true;
        [HideInInspector] public bool canInteract = true;
        [HideInInspector] public bool isRunning;
        public int currentAmmo;
        private CharacterController _characterController;
        private PlayerInputActions _inputActions;
        private Transform _playerCamera;
        private Vector3 _moveDirection;
        private Vector2 _moveInput;
        private float _rotationY;
        private float _reloadTime;
        private float _fireRateTime;
        private Transform _gunBarrelEnd;
        private Transform _gunBarrelEndThirdPerson;
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
                currentAmmo = currentWeapon.clipSize;
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

        [ObserversRpc]
        private void WeaponChange()
        {
            foreach (Transform child in gunObjectViewmodel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in thirdPersonGunObject.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject model = Instantiate(currentWeapon.weaponModel, gunObjectViewmodel.transform);
            GameObject thirdPersonModel = Instantiate(currentWeapon.thirdPersonModel, thirdPersonGunObject.transform);
            _reloadTime = 0;
            _fireRateTime = 0;
            _previousWeapon = currentWeapon;
            _gunBarrelEnd = model.transform.Find("Model").Find("BarrelEnd");
            _gunBarrelEndThirdPerson = thirdPersonModel.transform.Find("Model").Find("BarrelEnd");
        }
        
        private void EquippedWeapon()
        {
            if (currentWeapon == null) return;
            if (_reloadTime > 0)
            {
                _reloadTime -= Time.deltaTime;
                if (_reloadTime <= 0)
                {
                    currentAmmo = currentWeapon.clipSize;
                }
            }
            if (_fireRateTime > 0)
            {
                _fireRateTime -= Time.deltaTime;
            }
            if (_previousWeapon != currentWeapon)
            {
                WeaponChange();
            }
            
            if (!canShoot) return;
            
            if (_inputActions.Player.ItemReload.WasPressedThisFrame() && _reloadTime <= 0 && currentAmmo < currentWeapon.clipSize)
            {
                _reloadTime = currentWeapon.reloadTime;
            }
            
            if (_fireRateTime > 0 || _reloadTime > 0 || currentAmmo <= 0) return;
            
            switch (currentWeapon.weaponType)
            {
                case WeaponScriptableObject.WeaponType.None:
                    return;
                case WeaponScriptableObject.WeaponType.Ranged:
                    if (currentWeapon.fireMode == WeaponScriptableObject.FireMode.Single && _inputActions.Player.ItemPrimary.WasPressedThisFrame() && _fireRateTime <= 0)
                    {
                        ShootBullet(_playerCamera.position, _playerCamera.forward);
                        currentAmmo--;
                        _fireRateTime = currentWeapon.fireRate;
                        if (currentAmmo <= 0)
                        {
                            _reloadTime = currentWeapon.reloadTime;
                        }
                    }
                    else if (currentWeapon.fireMode == WeaponScriptableObject.FireMode.Automatic && _inputActions.Player.ItemPrimary.IsPressed() && _fireRateTime <= 0)
                    {
                        ShootBullet(_playerCamera.position, _playerCamera.forward);
                        currentAmmo--;
                        _fireRateTime = currentWeapon.fireRate;
                        if (currentAmmo <= 0)
                        {
                            _reloadTime = currentWeapon.reloadTime;
                        }
                    }
                    break;
                case WeaponScriptableObject.WeaponType.Melee:
                    return;
                default:
                    Debug.Log("Unknown weapon type equipped");
                    return;
            }
        }
        
        [ObserversRpc]
        private void ShootBullet(Vector3 rayOriginPosition, Vector3 rayOriginForward, Channel channel = Channel.Unreliable)
        {
            Debug.Log("Shot from " + base.Owner.ClientId);
            if (Physics.Raycast(rayOriginPosition, rayOriginForward, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject.TryGetComponent(out IShootable shootableObj) && base.IsOwner)
                {
                    shootableObj.Shoot(this);   
                }
            }

            GameObject bulletTrail = Instantiate(bulletTrailObject, base.IsOwner ? _gunBarrelEnd.position : _gunBarrelEndThirdPerson.position, Quaternion.identity);
            bulletTrail.transform.LookAt(hit.point);
            bulletTrail.transform.DOMove(hit.point, 0.075f).SetEase(Ease.Linear).OnComplete(() => Destroy(bulletTrail));
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
