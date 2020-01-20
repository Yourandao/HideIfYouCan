using System.Collections;

using Mirror;

using Scripts.PlayerScripts.Control;
using Scripts.Props;

using UnityEngine;

namespace Scripts.PlayerScripts.PlayerBehaviour
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
        public Transform modelHolder;

        [HideInInspector] public GameObject model;

        private GameObject propInstance;
        private Prop       prop;

        private float holdingTime;
        private bool  freezed;

        public void Setup(Player player)
        {
            this.player = player;

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
            {
                var propObject = hit.collider.gameObject;

                Transform(propObject);
                RpcTransform(propObject);
            }
        }

        [ClientRpc]
        private void RpcTransform(GameObject propObject) => Transform(propObject);

        private void Transform(GameObject propObject)
        {
            prop = propObject.GetComponent<PropHolder>().prop;

            if (isServer)
                NetworkServer.Destroy(model);

            Destroy(propInstance);
            propInstance = Instantiate(prop.prefab, modelHolder);

            int layer = Utility.LayerMaskToLayer(player.hiderMask);

            if (propInstance.layer != layer)
                Utility.SetLayerRecursively(propInstance, layer);

            if (!isLocalPlayer)
                return;

            controller.SwitchToTPP(propInstance.transform);
            controller.speedMultiplier      = prop.speedMultiplier;
            controller.jumpHeightMultiplier = prop.jumpHeightMultiplier;

            var size = propObject.GetComponent<MeshRenderer>().bounds.size;

            controller.SetSize(new Vector3(size.x / 2, size.y, size.z / 2));
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

            controller.SetFreeze(state, true);
        }
    }
}