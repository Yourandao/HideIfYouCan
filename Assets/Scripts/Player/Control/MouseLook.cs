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

        [SerializeField] private float minimumXThirdPerson = 0f;
        // TODO: Count it automatically

        private Transform player;
        private Transform firstPersonCamera;
        private Transform thirdPersonCamera;

        private Transform target;

        private bool firstPersonPerspective;

        private float mouseX;
        private float mouseY;

        private float xRotation;
        private float yRotation;

        [HideInInspector] public bool freezeModelRotation;

        public void Setup(Transform player, Transform firstPersonCamera)
        {
            this.player            = player;
            this.firstPersonCamera = firstPersonCamera;
        }

        public void SwitchToFPP()
        {
            target = null;

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
        }

        public void Rotate()
        {
            xRotation -= mouseY;
            yRotation += mouseX;

            firstPersonCamera.localRotation =
                Quaternion.Euler(Mathf.Clamp(xRotation, minimumXFirstPerson, maximumXFirstPerson), 0f, 0f);

            if (!freezeModelRotation)
                player.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (firstPersonPerspective)
                return;

            thirdPersonCamera.LookAt(target);

            target.rotation =
                Quaternion.Euler(Mathf.Clamp(xRotation, minimumXThirdPerson, maximumXThirdPerson), yRotation, 0f);
        }
    }
}