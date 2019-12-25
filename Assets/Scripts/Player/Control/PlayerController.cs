using Assets.Scripts.Exceptions;
using Assets.Scripts.PlayerScripts.PlayerRoles;
using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
	public class PlayerController : MonoBehaviour
	{
		private CharacterController _controller;

		private Vector3 _move = Vector3.zero;

		private Camera _camera;

		[SerializeField] private MouseLook _mouseLook = new MouseLook();

		[SerializeField] private float _gravity = 20.0f;

		[SerializeField] private float _acceleration = 1.0f;

		[Header("Cameras")]
		[SerializeField] private GameObject firstPersonCamera = default;
		private GameObject firstPersonCameraInstance;

		[SerializeField] private GameObject thirdPersonCameraController = default;

		[SerializeField] private GameObject thirdPersonCameraPrefab = default;
		private GameObject thirdPersonCameraInstance;
		private GameObject thirdPersonCameraControllerInstance;

		public float Speed = 10f;

		public float JumpSpeed = 8.0f;

		public void ChangeMode(Roles role)
		{
			Destroy(thirdPersonCameraInstance);
			Destroy(thirdPersonCameraControllerInstance);
			Destroy(firstPersonCameraInstance);

			switch (role)
			{
				case Roles.Hider:
					thirdPersonCameraControllerInstance = Instantiate(thirdPersonCameraController, transform);
					thirdPersonCameraControllerInstance.name = thirdPersonCameraController.name;

					thirdPersonCameraInstance = Instantiate(thirdPersonCameraPrefab);
					thirdPersonCameraInstance.name = thirdPersonCameraPrefab.name;

					break;

				case Roles.Seeker:
					firstPersonCameraInstance = Instantiate(firstPersonCamera, transform);
					firstPersonCameraInstance.name = firstPersonCamera.name;

					break;

				default: throw new UnhandledRoleException(role);
			}

			var currentCamera = role == Roles.Seeker ? firstPersonCameraInstance : thirdPersonCameraInstance;
			_camera = currentCamera.GetComponent<Camera>();

			_mouseLook.Setup(transform, _camera.transform);
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
		}

		private void Update()
		{
			if (Cursor.lockState != CursorLockMode.Locked)
				Cursor.lockState = CursorLockMode.Locked;

			_mouseLook.Rotate(transform, _camera.transform);

			if (_controller.isGrounded)
			{
				_move = (transform.right * Input.GetAxisRaw("Horizontal")
				         + transform.forward * Input.GetAxisRaw("Vertical")) * Speed;

				if (Input.GetKey(KeyCode.LeftShift))
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
	}
}
