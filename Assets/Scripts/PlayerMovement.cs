using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private CharacterController _controller;

	private Vector3 _move = Vector3.zero;

	[SerializeField] [Range(.0f, 20.0f)] private float _speed;

	[SerializeField] [Range(.0f, 20.0f)] private float _jumpSpeed = 8.0f;
	
	[SerializeField] [Range(.0f, 50.0f)] private float _gravity = 20.0f;

	[SerializeField] [Range(1.0f, 5.0f)] private float _acceleration = 1.0f;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (_controller.isGrounded)
		{ 
			_move = (transform.right * Input.GetAxis("Horizontal") 
								+ transform.forward * Input.GetAxis("Vertical")) * _speed;

			if (Input.GetKey(KeyCode.LeftShift))
			{
				_move.x *= _acceleration;
				_move.z *= _acceleration;
			}

			if (Input.GetButton("Jump"))
			{
				_move.y = _jumpSpeed;
			}
		}

		_move.y -= _gravity * Time.deltaTime;

		_controller.Move(_move * Time.deltaTime);
	}
}
