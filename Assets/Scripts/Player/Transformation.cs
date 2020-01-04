using Mirror;

using Scripts.Components;
using Scripts.Management.Network;
using Scripts.PlayerScripts.Control;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    public sealed class Transformation : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private Player player;

        [SerializeField] private PlayerController controller;

        [Header("Settings")]
        [SerializeField] private float interactionDistance = 5f;

        [SerializeField] private LayerMask interactableObjects;

        private GameObject modelInstance;

        [Header("Prop Components")]
        [SerializeField] private GameObject seekerModel;

        [SerializeField] private Transform modelHolder;

        private Prop prop;

        private void Update()
        {
            if (controller.Freezed)
                return;

            if (Input.GetButtonDown("Interact") && player.role == Role.Hider)
                CmdTransform(controller.CurrentCamera.transform.position, controller.CurrentCamera.transform.forward);
        }

        [Command]
        private void CmdTransform(Vector3 from, Vector3 direction)
        {
            if (Physics.Raycast(from, direction, interactionDistance, interactableObjects))
                ServerManager.GetPlayer(netId).transformation.RpcTransform(from, direction);
        }

        [ClientRpc]
        private void RpcTransform(Vector3 from, Vector3 direction)
        {
            Physics.Raycast(from, direction, out var hit,
                            interactionDistance, interactableObjects);

            prop = hit.collider.GetComponent<PropController>().prop;

            seekerModel.SetActive(false);

            Destroy(modelInstance);

            modelInstance = Instantiate(prop.prefab, modelHolder);

            if (!isLocalPlayer)
                return;

            controller.Configure(false);
            controller.speedMultiplier     = prop.speedMultiplier;
            controller.jumpForceMultiplier = prop.jumpForceMultiplier;
        }
    }
}