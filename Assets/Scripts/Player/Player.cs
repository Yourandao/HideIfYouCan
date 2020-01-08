using System;

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
        [Header("Components")]
        public PlayerController controller;

        private UserInterface userInterface;

        public Transformation transformation;

        [HideInInspector]
        [SyncVar] public Role role;

        public void Setup(UserInterface userInterface)
        {
            this.userInterface = userInterface;

            this.userInterface.player = this;
            this.userInterface.UpdateStats();

            CmdSetup();
        }

        [Command]
        private void CmdSetup() => RpcSetup();

        [ClientRpc]
        private void RpcSetup()
        {
            if (role != Role.Hider)
                transformation.enabled = false;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                userInterface.TogglePause();
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            controller.freezed = false;
        }

        [ClientRpc]
        public void RpcStopGame()
        {
            controller.SetStop(true);
        }
    }
}