using Assets.Scripts.PlayerScripts.PlayerRoles;

using Mirror;

namespace Assets.Scripts.PlayerScripts
{
    public class Player : NetworkBehaviour
    {
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
            Utility.ToggleComponents(ref playerRole.defaultRoleSet, false);
        }

        private void OnGameStartCallback()
        {
            var roleSet = playerRole.GetRoleSet();

            Utility.ToggleComponents(ref roleSet, true);
        }
    }
}