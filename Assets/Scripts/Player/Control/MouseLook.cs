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

        private Transform player;
        private Transform firstPersonCamera;
        private Transform thirdPersonCamera;

        private Transform target;

        private bool firstPersonPerspective;

        private float mouseX;
        private float mouseY;

        private float xRotation;
        private float yRotation;

        private float fov;

        [HideInInspector] public bool freezeModelRotation;

        public void Setup(Transform player, Transform firstPersonCamera)
        {
            this.player            = player;
            this.firstPersonCamera = firstPersonCamera;

            firstPersonPerspective = true;
        }
        
        public void SwitchToTPP(Transform thirdPersonCamera, Transform target)
        {
            this.thirdPersonCamera = thirdPersonCamera;
            this.target            = target;

            firstPersonPerspective = false;
        }

        public void InputRotation()
        {
            mouseX = Input.GetAxisRaw("Mouse X") * xSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * ySensitivity;

            fov += Input.GetAxisRaw("MouseWheel") * mouseWheelSensivity;
        }

        public void Rotate()
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minimumXFirstPerson, maximumXFirstPerson);

            yRotation += mouseX;

            firstPersonCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (!freezeModelRotation)
                player.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (firstPersonPerspective)
                return;

            fov = Mathf.Clamp(fov, minFov, maxFov);

            thirdPersonCamera.LookAt(target);
            thirdPersonCamera.GetComponent<Camera>().fieldOfView = fov;

            target.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}