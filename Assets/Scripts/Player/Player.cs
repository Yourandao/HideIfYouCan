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
                controller.ChangeCameraMode(role);
            }
        }
    }
}