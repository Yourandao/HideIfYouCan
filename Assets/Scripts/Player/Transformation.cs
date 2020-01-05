using System.Collections;
using System.Collections.Generic;

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

        [SerializeField] private float holdToFreeze = 2f;

        [SerializeField] private float freezingTime = 3f;

        [SerializeField] private LayerMask interactableObjects;

        private GameObject modelInstance;

        [Header("Prop Components")]
        [SerializeField] private GameObject seekerModel;

        [SerializeField] private Transform modelHolder;

        private Prop prop;

        private float holdingTime;

        private bool freezed;

        private void Update()
        {
            if (controller.freezed)
                return;

            if (Input.GetButton("Interact") && !freezed)
                holdingTime += Time.deltaTime;

            if (!Input.GetButtonUp("Interact"))
                return;

            if (holdingTime < holdToFreeze)
                CmdTransform(controller.CurrentCamera.transform.position,
                             controller.CurrentCamera.transform.forward);
            else
                CmdSetFreeze(freezed);

            holdingTime = 0f;
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

            Utility.SetLayerRecursively(hit.collider.gameObject, 0);

            prop = hit.collider.GetComponent<PropHolder>().prop;

            seekerModel.SetActive(false);

            Destroy(modelInstance);

            modelInstance = Instantiate(prop.prefab, modelHolder);

            if (!isLocalPlayer)
                return;

            controller.Configure(false);
            controller.speedMultiplier     = prop.speedMultiplier;
            controller.jumpForceMultiplier = prop.jumpForceMultiplier;
        }

        [Command]
        private void CmdSetFreeze(bool state) => StartCoroutine(DelayedFreeze(state));

        private IEnumerator DelayedFreeze(bool state)
        {
            if (!state)
                RpcSetFreeze(false);
            else
            {
                float time = 0f;

                while (time < freezingTime)
                {
                    yield return new WaitForFixedUpdate();

                    time += Time.fixedDeltaTime;
                }

                RpcSetFreeze(true);
            }
        }

        [ClientRpc]
        private void RpcSetFreeze(bool state)
        {
            freezed = state;

            controller.SetFreeze(state);
        }
    }
}