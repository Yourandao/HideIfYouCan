using Mirror;

using Scripts.Components;
using Scripts.PlayerScripts.Control;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    public sealed class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerController controller;

        [HideInInspector]
        [SyncVar] public Role role;

        private bool canMove;

        [SerializeField] private Transform modelHolder;

        [SerializeField] private GameObject seekerModel;

        private GameObject modelInstance;

        private Prop prop;

        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 10f;

        [SerializeField] private LayerMask interactableObjects;

        public void Setup()
        {
            CmdSetup();
        }

        [Command]
        private void CmdSetup() => RpcSetup();

        [ClientRpc]
        private void RpcSetup()
        {
            if (isLocalPlayer)
                controller.Configure(true);
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            canMove = true;

            controller.CanMove = true;
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if (!canMove)
                return;

            if (Input.GetButtonDown("Interact") && role == Role.Hider)
                Transform();
        }

        [Client]
        private void Transform() => RpcTransform();

        [Command]
        private void CmdTransform()
        {
            if (Physics.Raycast(controller.CurrentCamera.transform.position, controller.CurrentCamera.transform.forward,
                                interactionDistance, interactableObjects))
            {
                RpcTransform();
            }
        }

        [ClientRpc]
        private void RpcTransform()
        {
            Physics.Raycast(controller.CurrentCamera.transform.position, controller.CurrentCamera.transform.forward,
                            out var hit, interactionDistance, interactableObjects);

            prop = hit.transform.GetComponent<PropController>().prop;

            seekerModel.SetActive(false);
            Destroy(modelInstance);

            modelInstance = Instantiate(prop.prefab, modelHolder);

            controller.speedMultiplier     = prop.speedMultiplier;
            controller.jumpForceMultiplier = prop.jumpForceMultiplier;

            controller.Configure(false);
        }
    }
}