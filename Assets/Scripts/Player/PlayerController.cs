using FishNet.Object;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float gravity;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private GameObject firstPersonAssets;
        [SerializeField] private GameObject thirdPersonAssets;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool isRunning;
        private CharacterController _characterController;
        private Camera playerCamera;
        private Vector3 _moveDirection;
        private float _rotationY;

        public override void OnStartClient()
        {
            if (base.IsOwner)
            {
                playerCamera = Camera.main;
                playerCamera.transform.SetParent(transform);
                playerCamera.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                firstPersonAssets.SetActive(true);
                thirdPersonAssets.SetActive(false);
            }
            else
            {
                firstPersonAssets.SetActive(false);
                thirdPersonAssets.SetActive(true);
                this.enabled = false;
            }
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float vertical = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float horizontal = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float moveDirectionY = _moveDirection.y;
            _moveDirection = (forward * vertical) + (right * horizontal);

            if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
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

            _characterController.Move(_moveDirection * Time.deltaTime);

            if (canMove && playerCamera != null)
            {
                _rotationY += -Input.GetAxis("Mouse Y") * mouseSensitivity;
                _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationY, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
            }
        }
    }
}
