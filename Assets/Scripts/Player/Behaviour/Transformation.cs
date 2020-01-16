using System.Collections;

using Mirror;

using Scripts.Management.Network;
using Scripts.PlayerScripts.Control;
using Scripts.Props;

using UnityEngine;

namespace Scripts.PlayerScripts.Behaviour
{
    public sealed class Transformation : NetworkBehaviour
    {
        private Player player;

        private PlayerController controller;

        [Header("Settings")]
        [SerializeField] private float interactionDistance = 5f;

        [SerializeField] private float freezeHoldingTime = 2f;
        [SerializeField] private float freezingTime      = 3f;

        [SerializeField] private LayerMask interactableObjects;

        [Header("Prop Components")]
        [SerializeField] private GameObject seekerModel;

        [SerializeField] private Transform modelHolder;

        private Prop prop;

        private GameObject modelInstance;

        private float holdingTime;

        private bool freezed;

        private void Start()
        {
            player = GetComponent<Player>();
            
            controller = player.controller;
        }

        private void Update()
        {
            if (player.Paused)
                return;

            if (Input.GetButton("Interact"))
                holdingTime += Time.deltaTime;

            if (holdingTime >= freezeHoldingTime && prop != null)
            {
                CmdSetFreeze(!freezed);

                holdingTime = 0f;
            }
            else if (Input.GetButtonUp("Interact"))
            {
                holdingTime = 0f;

                if (!freezed)
                    CmdTransform();
            }
        }

        [Command]
        private void CmdTransform()
        {
            var from      = controller.firstPersonCamera.position;
            var direction = controller.firstPersonCamera.forward;

            if (Physics.Raycast(from, direction, out var hit, interactionDistance, interactableObjects))
                TargetTransform(connectionToClient, hit.collider.gameObject);
        }

        [TargetRpc]
        private void TargetTransform(NetworkConnection connection, GameObject propObject)
        {
            prop = propObject.GetComponent<PropHolder>().prop;

            seekerModel.SetActive(false);

            Destroy(modelInstance);

            modelInstance = Instantiate(prop.prefab, modelHolder);

            Utility.SetLayerRecursively(modelInstance, Utility.LayerMaskToLayer(player.propMask));

            if (!isLocalPlayer)
                return;

            controller.SwitchToTPP(modelInstance.transform);
            controller.speedMultiplier      = prop.speedMultiplier;
            controller.jumpHeightMultiplier = prop.jumpHeightMultiplier;

            // TODO: Configure character controller size according to prop size
        }

        [Command]
        private void CmdSetFreeze(bool state) => StartCoroutine(DelayedFreeze(state));

        private IEnumerator DelayedFreeze(bool state)
        {
            if (!state)
                RpcSetFreeze(false);
            else
            {
                yield return new WaitForSeconds(freezingTime);

                RpcSetFreeze(true);
            }
        }

        [ClientRpc]
        private void RpcSetFreeze(bool state)
        {
            freezed = state;

            controller.SetPropFreeze(state);
        }
    }
}