using Assets.Scripts.Exceptions;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller = default;

        private Vector3 _move = Vector3.zero;

        private Camera _camera;

        [Header("Settings")]
        [SerializeField] private MouseLook _mouseLook = new MouseLook();

        [SerializeField] private float _gravity = 20.0f;

        [SerializeField] private float _acceleration = 1.0f;

        public float Speed = 10f;

        public float JumpSpeed = 8.0f;

        [Header("Cameras")]
        [SerializeField] private GameObject firstPersonCamera = default;

        [SerializeField] private GameObject thirdPersonCameraController = default;

        [SerializeField] private GameObject thirdPersonCameraPrefab = default;
        private                  GameObject thirdPersonCameraInstance;

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;

            if (_controller.isGrounded)
            {
                _move = (transform.right * Input.GetAxisRaw("Horizontal")
                         + transform.forward * Input.GetAxisRaw("Vertical")) * Speed;

                if (Input.GetAxisRaw("SpeedModificator") == 1f)
                {
                    _move.x *= _acceleration;
                    _move.z *= _acceleration;
                }

                if (Input.GetButton("Jump"))
                {
                    _move.y = JumpSpeed;
                }
            }

            _move.y -= _gravity * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            _mouseLook.Rotate(transform, _camera.transform);
            _controller.Move(_move * Time.fixedDeltaTime);
        }

        public void ChangeCameraMode(Role role)
        {
            Destroy(thirdPersonCameraInstance);

            firstPersonCamera.SetActive(false);
            thirdPersonCameraController.SetActive(false);

            switch (role)
            {
                case Role.Hider:
                    thirdPersonCameraInstance      = Instantiate(thirdPersonCameraPrefab);
                    thirdPersonCameraInstance.name = thirdPersonCameraPrefab.name;

                    thirdPersonCameraController.SetActive(true);

                    break;

                case Role.Seeker:
                    firstPersonCamera.SetActive(true);

                    break;

                default: throw new UnhandledRoleException(role);
            }

            var currentCamera = role == Role.Seeker ? firstPersonCamera : thirdPersonCameraInstance;
            _camera = currentCamera.GetComponent<Camera>();

            _mouseLook.Setup(transform, _camera.transform);
        }
    }
}