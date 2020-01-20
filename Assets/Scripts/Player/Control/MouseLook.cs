using System;

using UnityEngine;

namespace Scripts.PlayerScripts.Control
{
    [Serializable]
    public sealed class MouseLook
    {
        [SerializeField] private float xSensitivity = 2.5f;
        [SerializeField] private float ySensitivity = 2.5f;

        [Header("FPP")]
        [SerializeField] private float maximumXFirstPerson = 90f;

        [SerializeField] private float minimumXFirstPerson = -90f;
        
        [Header("TPP")]
        [SerializeField] private float maximumXThirdPerson = 60f;

        [SerializeField] private float minimumXThirdPerson = -15f;
        // TODO: Count it automatically

        [SerializeField] private float minFov = 10f;
        [SerializeField] private float maxFov = 20f;

        [SerializeField] private float mouseWheelSensivity = .01f;

        private Transform playerTransform;
        private Transform firstPersonCameraTransform;
        private Transform thirdPersonCameraTransform;

        private Camera thirdPersonCamera;

        private Transform target;

        private bool firstPersonPerspective;

        private float mouseX;
        private float mouseY;

        private float xRotation;
        private float yRotation;

        private float fov;

        [HideInInspector] public bool freezeModelRotation;

        public void Setup(Transform playerTransform, Transform firstPersonCameraTransform)
        {
            this.playerTransform            = playerTransform;
            this.firstPersonCameraTransform = firstPersonCameraTransform;

            firstPersonPerspective = true;

            xRotation = firstPersonCameraTransform.localRotation.eulerAngles.x;
            yRotation = playerTransform.rotation.eulerAngles.y;
        }

        public void SwitchToTPP(Transform thirdPersonCameraTransform, Transform target)
        {
            this.thirdPersonCameraTransform = thirdPersonCameraTransform;
            this.target                     = target;

            thirdPersonCamera = this.thirdPersonCameraTransform.GetComponent<Camera>();

            firstPersonPerspective = false;
        }

        public void InputRotation()
        {
            mouseX = Input.GetAxis("Mouse X") * xSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * ySensitivity;

            fov += Input.GetAxis("MouseWheel") * mouseWheelSensivity;
        }

        public void Rotate()
        {
            xRotation -= mouseY;
            xRotation =  Mathf.Clamp(xRotation, minimumXFirstPerson, maximumXFirstPerson);

            yRotation += mouseX;

            firstPersonCameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (!freezeModelRotation)
                playerTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (firstPersonPerspective)
                return;

            fov = Mathf.Clamp(fov, minFov, maxFov);

            thirdPersonCameraTransform.LookAt(target);
            thirdPersonCamera.fieldOfView = fov;

            target.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}