using System;

using UnityEngine;

namespace Scripts.PlayerScripts.Control
{
    [Serializable]
    public sealed class MouseLook
    {
        [SerializeField] private float xSensitivity = 2.5f;
        [SerializeField] private float ySensitivity = 2.5f;

        [SerializeField] private float maximumX = 90f;
        [SerializeField] private float minimumX = -90f;

        private Transform playerTransform;
        private Transform cameraTransform;

        private float mouseX;
        private float mouseY;

        private float xRotation;

        private bool isSetUp;

        public bool freezeModelRotation;

        public void Setup(Transform player, Transform camera)
        {
            playerTransform = player;
            cameraTransform = camera;

            isSetUp = true;
        }

        public void InputRotation()
        {
            mouseX = Input.GetAxisRaw("Mouse X") * xSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * ySensitivity;
        }

        public void Rotate()
        {
            if (!isSetUp)
                return;

            xRotation -= mouseY;
            xRotation =  Mathf.Clamp(xRotation, minimumX, maximumX);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (!freezeModelRotation)
                playerTransform.Rotate(Vector3.up * mouseX);
        }
    }
}