using Assets.Scripts.Managing.Game;
using Assets.Scripts.PlayerScripts.PlayerRoles;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public class Player : NetworkBehaviour
    {
        public PlayerRole playerRole = new PlayerRole();

        [SerializeField] private Behaviour[] playerTools;

        public void Setup()
        {
            CmdSetup();
        }

        [Command]
        private void CmdSetup() =>  RpcSetup();

        [ClientRpc]
        private void RpcSetup() => SetDefaults();

        private void SetDefaults()
        {
            Utility.ToggleComponents(ref playerTools, false);

            GameManager.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            var roleSet = playerRole.GetRoleSet();

            Utility.ToggleComponents(ref roleSet, true);
        }
    }
}