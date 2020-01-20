using UnityEngine;

namespace Scripts.PlayerScripts.Control
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        private Player player;

        [SerializeField] private CharacterController controller;

        private Animator animator;

        private static readonly int _isMoving   = Animator.StringToHash("IsMoving");
        private static readonly int _horizontal = Animator.StringToHash("Horizontal");
        private static readonly int _vertical   = Animator.StringToHash("Vertical");
        private static readonly int _isRunning  = Animator.StringToHash("IsRunning");

        [Header("Cameras")]
        public Transform firstPersonCamera;

        [SerializeField] private GameObject thirdPersonCameraPrefab;
        [SerializeField] private Vector3    thirdPersonCameraOffset;

        private GameObject cameraTargetInstance;
        private GameObject thirdPersonCameraInstance;

        [SerializeField] private MouseLook mouseLook = new MouseLook();

        [Header("Settings")]
        [SerializeField] private float jogSpeed = 2.5f;

        [SerializeField] private float runSpeed   = 5f;
        [SerializeField] private float jumpHeight = 1f;

        [SerializeField] [Range(0f, 1f)] private float smoothFactor = .2f;

        [HideInInspector] public float speedMultiplier      = 1f;
        [HideInInspector] public float jumpHeightMultiplier = 1f;

        private bool freezed;
        private bool stopped;

        private bool jumpEnabled;

        private float speed;

        private Vector3 input;

        private Vector3 velocity;
        private Vector3 localVelocity;

        public void Setup(Animator animator)
        {
            player = GetComponent<Player>();

            this.animator = animator;
            ResetAnimator();

            mouseLook.Setup(transform, firstPersonCamera);

            Cursor.lockState = CursorLockMode.Locked;

            freezed = true;

            speed = jogSpeed;

            input = new Vector3();
        }

        private void ResetAnimator()
        {
            animator.SetFloat(_horizontal, 0f);
            animator.SetFloat(_vertical, 0f);

            animator.SetBool(_isMoving, false);
            animator.SetBool(_isRunning, false);
        }

        private void Update()
        {
            if (stopped)
                return;

            mouseLook.InputRotation();

            if (freezed)
                return;

            bool runState = Input.GetButton("Run");

            speed = runState ? runSpeed : jogSpeed;

            input.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            if (controller.isGrounded)
            {
                if (Input.GetButtonDown("Jump") && jumpEnabled)
                    velocity.y = Mathf.Sqrt(jumpHeight * jumpHeightMultiplier * -2f * Physics.gravity.y);
            }
            else
                velocity += Physics.gravity * Time.deltaTime;

            if (animator != null)
            {
                animator.SetBool(_isRunning, runState);
                animator.SetBool(_isMoving, input != Vector3.zero);
            }
        }

        private void FixedUpdate()
        {
            var desiredVelocity = transform.right * input.x +
                                  transform.forward * input.z;

            float y = velocity.y;

            velocity = Vector3.Lerp(velocity,
                                    desiredVelocity * (speed * speedMultiplier),
                                    smoothFactor);

            velocity.y = y;

            localVelocity = Vector3.Lerp(localVelocity, input, smoothFactor);

            controller.Move(velocity * Time.fixedDeltaTime);

            if (animator != null)
            {
                animator.SetFloat(_horizontal, localVelocity.x);
                animator.SetFloat(_vertical, localVelocity.z);
            }
        }

        private void LateUpdate()
        {
            if (stopped)
                return;

            mouseLook.Rotate();
        }

        public void SwitchToTPP(Transform model)
        {
            firstPersonCamera.gameObject.SetActive(false);

            jumpEnabled = true;

            Destroy(thirdPersonCameraInstance);
            Destroy(cameraTargetInstance);

            cameraTargetInstance = new GameObject("CameraTarget");
            cameraTargetInstance.transform.SetParent(model);
            cameraTargetInstance.transform.SetPositionAndRotation(model.position, model.rotation);
            cameraTargetInstance.AddComponent<AudioListener>();

            thirdPersonCameraInstance =
                Instantiate(thirdPersonCameraPrefab, cameraTargetInstance.transform);

            thirdPersonCameraInstance.transform.localPosition = thirdPersonCameraOffset;
            thirdPersonCameraInstance.name                    = thirdPersonCameraPrefab.name;

            mouseLook.SwitchToTPP(thirdPersonCameraInstance.transform, cameraTargetInstance.transform);
        }

        public void SetFreeze(bool state, bool propOnly)
        {
            freezed = state;

            if (propOnly)
                mouseLook.freezeModelRotation = state;

            if (state)
                input = Vector3.zero;
        }

        public void SetStop(bool state)
        {
            stopped = state;

            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;

            if (state)
                input = Vector3.zero;
        }

        public void SetSize(Vector3 size)
        {
            controller.height = size.y;
            controller.center = new Vector3(0f, controller.height / 2, 0f);

            controller.radius = size.x <= size.z ? size.x : size.z;
        }

        private void OnEnable() => controller.enabled = true;

        private void OnDisable()
        {
            controller.enabled = false;

            Destroy(thirdPersonCameraInstance);
            Destroy(cameraTargetInstance);
        }
    }
}