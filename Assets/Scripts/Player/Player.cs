using Mirror;

using Scripts.Components;
using Scripts.PlayerScripts.Control;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Transformation))]
    public sealed class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerController controller;

        public Transformation transformation;

        [HideInInspector]
        [SyncVar] public Role role;

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

            if (role != Role.Hider)
                transformation.enabled = false;
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            controller.freezed = false;
        }

        [ClientRpc]
        public void RpcStopGame()
        {
            controller.stopped = true;
        }
    }
}