using Mirror;

using UnityEngine;

namespace Scripts.PlayerScripts.Control
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        [SerializeField] private Animator        animator;
        [SerializeField] private NetworkAnimator networkAnimator;

        private static readonly int _moving     = Animator.StringToHash("Moving");
        private static readonly int _horizontal = Animator.StringToHash("Horizontal");
        private static readonly int _vertical   = Animator.StringToHash("Vertical");
        private static readonly int _isRunning  = Animator.StringToHash("IsRunning");

        [Header("Cameras")]
        [SerializeField] private GameObject firstPersonCamera;

        [SerializeField] private GameObject thirdPersonCameraController;

        [SerializeField] private GameObject thirdPersonCameraPrefab;
        private                  GameObject thirdPersonCameraInstance;

        public Camera CurrentCamera { get; private set; }

        [Header("Movement")]
        [SerializeField] private MouseLook mouseLook = new MouseLook();

        [SerializeField] private float jogSpeed  = 2.5f;
        [SerializeField] private float runSpeed  = 5f;
        [SerializeField] private float jumpForce = 5f;

        [SerializeField] [Range(0f, 1f)] private float smoothFactor = .25f;

        [HideInInspector] public float speedMultiplier     = 1f;
        [HideInInspector] public float jumpForceMultiplier = 1f;

        public bool freezed;
        public bool stopped;

        private bool jumpEnabled;

        private float speed;

        private Vector3 input;

        private Vector3 velocity;
        private Vector3 localVelocity;

        private void Start()
        {
            animator.SetFloat(_horizontal, 0f);
            animator.SetFloat(_vertical, 0f);

            freezed = true;

            speed = jogSpeed;

            input = new Vector3();

            velocity      = new Vector3();
            localVelocity = new Vector3();
        }

        private void Update()
        {
            if (stopped)
                return;

            //if (Cursor.lockState != CursorLockMode.Locked)
            //    Cursor.lockState = CursorLockMode.Locked;

            mouseLook.InputRotation();

            if (freezed)
                return;

            if (controller.isGrounded)
            {
                if (Input.GetButtonDown("Run"))
                {
                    speed = runSpeed;

                    animator.SetBool(_isRunning, true);
                }
                else if (Input.GetButtonUp("Run"))
                {
                    speed = jogSpeed;

                    animator.SetBool(_isRunning, false);
                }

                input.Set(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

                if (Input.GetButtonDown("Jump") && jumpEnabled)
                    velocity.y = jumpForce * jumpForceMultiplier;
            }
            else
                velocity += Physics.gravity * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            mouseLook.Rotate();

            var desiredVelocity = transform.right * input.x +
                                  transform.forward * input.z;

            velocity = Vector3.Lerp(velocity,
                                    desiredVelocity * speed * speedMultiplier,
                                    smoothFactor);

            localVelocity = Vector3.Lerp(localVelocity, input, smoothFactor);

            controller.Move(velocity * Time.fixedDeltaTime);

            animator.SetBool(_moving, desiredVelocity != Vector3.zero);

            animator.SetFloat(_horizontal, localVelocity.x);
            animator.SetFloat(_vertical, localVelocity.z);
        }

        public void Configure(bool firstPerson)
        {
            Destroy(thirdPersonCameraInstance);

            firstPersonCamera.SetActive(false);
            thirdPersonCameraController.SetActive(false);

            networkAnimator.enabled = false;
            animator.enabled        = false;

            jumpEnabled = false;

            if (firstPerson)
            {
                firstPersonCamera.SetActive(true);

                networkAnimator.enabled = true;
                animator.enabled        = true;
            }
            else
            {
                thirdPersonCameraInstance      = Instantiate(thirdPersonCameraPrefab);
                thirdPersonCameraInstance.name = thirdPersonCameraPrefab.name;

                thirdPersonCameraController.SetActive(true);

                jumpEnabled = true;
            }

            var cameraObject = firstPerson ? firstPersonCamera : thirdPersonCameraInstance;
            CurrentCamera = cameraObject.GetComponent<Camera>();

            mouseLook.Setup(transform, CurrentCamera.transform);
        }

        public void SetFreeze(bool state)
        {
            freezed                       = state;
            mouseLook.freezeModelRotation = state;
        }
    }
}