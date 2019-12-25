using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
	public class PlayerController : MonoBehaviour
	{
		private CharacterController _controller;

		private Vector3 _move = Vector3.zero;

		[SerializeField] private MouseLook _mouseLook = new MouseLook();

		[SerializeField] private Camera _camera;

		[SerializeField] private float _gravity = 20.0f;

		[SerializeField] private float _acceleration = 1.0f;

		public float Speed = 10f;

		public float JumpSpeed = 8.0f;

		private void Start()
		{
			_controller = GetComponent<CharacterController>();

			_mouseLook.Setup(transform, _camera.transform);
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
