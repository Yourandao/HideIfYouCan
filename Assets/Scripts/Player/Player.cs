using Assets.Scripts.Exceptions;
using Assets.Scripts.PlayerScripts.Control;
using Assets.Scripts.PlayerScripts.PlayerRoles;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public sealed class Player : NetworkBehaviour
    {
	    [SerializeField] private PlayerController _controller;

        public PlayerRole playerRole = new PlayerRole();

        public void Setup()
        {
            CmdSetup();
        }

        [Command]
        private void CmdSetup() => RpcSetup();

        [ClientRpc]
        private void RpcSetup() => SetDefaults();

        private void SetDefaults()
        {
            if (isLocalPlayer)
            {
				_controller.ChangeMode(playerRole.role);
            }

            var roleSet = playerRole.GetRoleSet();
            Utility.ToggleComponents(ref roleSet, true);

            Utility.ToggleComponents(ref playerRole.defaultRoleSet, false);
        }
    }
}