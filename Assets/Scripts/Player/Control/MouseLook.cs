using System;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
	[Serializable]
	public class MouseLook
	{
		private Transform _playerTransform;
		private Transform _cameraTransform;

		private float _xRotation;

		[Header("Sensitivity")] [SerializeField] [Range(10.0f, 200.0f)] private float _sensitivity = 100f;

		public void Setup(Transform player, Transform camera)
		{
			_playerTransform = player;
			_cameraTransform = camera;
		}

		public void Rotate(Transform player, Transform camera)
		{
			_playerTransform = player;
			_cameraTransform = camera;

			var mouseX = Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime;
			var mouseY = Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime;

			_xRotation -= mouseY;
			_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

			_cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
			_playerTransform.Rotate(Vector3.up * mouseX);
		}
	}
}