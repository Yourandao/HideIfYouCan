using System.Collections;

using Mirror;

using Scripts.Components;
using Scripts.Management.Network;
using Scripts.PlayerScripts.Control;
using Scripts.PlayerScripts.Settings;

using UnityEngine;

namespace Scripts.PlayerScripts.Behaviour
{
    public sealed class Transformation : NetworkBehaviour
    {
        [HideInInspector] public Player player;

        private PlayerController controller;

        private HiderSettings settings;

        [SerializeField] private LayerMask interactableObjects;

        private GameObject modelInstance;

        [Header("Prop Components")]
        [SerializeField] private GameObject seekerModel;

        [SerializeField] private Transform modelHolder;

        private Prop prop;

        private float holdingTime;

        private bool freezed;

        private void Start()
        {
            controller = player.controller;

            settings = player.gameSettings.hiderSettings;
        }

        private void Update()
        {
            if (player.Paused)
                return;

            if (Input.GetButton("Interact"))
                holdingTime += Time.deltaTime;

            if (holdingTime >= settings.freezeHoldingTime && prop != null)
            {
                CmdSetFreeze(!freezed);

                holdingTime = 0f;
            }
            else if (Input.GetButtonUp("Interact"))
            {
                holdingTime = 0f;

                if (!freezed)
                    CmdTransform(controller.firstPersonCamera.position,
                                 controller.firstPersonCamera.forward);
            }
        }

        [Command]
        private void CmdTransform(Vector3 from, Vector3 direction)
        {
            if (Physics.Raycast(from, direction, settings.interactionDistance, interactableObjects))
                ServerManager.GetPlayer(netId).transformation.RpcTransform(from, direction);
        }

        [ClientRpc]
        private void RpcTransform(Vector3 from, Vector3 direction)
        {
            Physics.Raycast(from, direction, out var hit,
                            settings.interactionDistance, interactableObjects);

            prop = hit.collider.GetComponent<PropHolder>().prop;

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
                float time = 0f;

                while (time < settings.freezingTime)
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

            controller.SetPropFreeze(state);
        }
    }
}