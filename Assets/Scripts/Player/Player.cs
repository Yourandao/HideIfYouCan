using Assets.Scripts.PlayerScripts.Control;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public sealed class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerController controller = default;

        [HideInInspector]
        [SyncVar] public Role role;

        [SerializeField] private Behaviour[] enabledDuringGame;

        public void Setup()
        {
            CmdSetup();
        }

        [Command]
        private void CmdSetup()
        {
            RpcSetup();
        }

        [ClientRpc]
        private void RpcSetup()
        {
            if (isLocalPlayer)
            {
                controller.ChangeCameraMode(role);
            }

            Utility.ToggleComponents(ref enabledDuringGame, false);
        }

        [Command]
        public void CmdStartGame()
        {
            RpcStartGame();
        }

        [ClientRpc]
        private void RpcStartGame()
        {
            Utility.ToggleComponents(ref enabledDuringGame, true);
        }

        private void Update()
        {
            // TODO: Send ready state
        }
    }
}