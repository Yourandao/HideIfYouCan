using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
	public class MouseLook : MonoBehaviour
	{
		[SerializeField] private Transform _playerTransform;

		private float _xRotation;

		[Header("Sensitivity")] [SerializeField] [Range(10.0f, 200.0f)] private float _sensitivity = 100f;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			var mouseX = Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime;
			var mouseY = Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime;

			_xRotation -= mouseY;
			_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

			transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
			_playerTransform.Rotate(Vector3.up * mouseX);
		}
	}
}