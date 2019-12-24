using Assets.Scripts.Managing.Game;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    [RequireComponent(typeof(Player))]
    public sealed class Setup : NetworkBehaviour
    {
        [SerializeField] private Player player = default;

        [Header("Components Management")]
        [SerializeField] private Behaviour[] componentsToEnable;

        [SerializeField] private CharacterController controller;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);
            controller.enabled = true;
    
            GetComponent<CharacterController>().enabled = true;

            player.Setup();
            CmdSetName(name);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            name = GetComponent<NetworkIdentity>().netId.ToString();

            GameManager.Players.Add(name, player);
        }

        [Command]
        private void CmdSetName(string newName) => name = newName;

        public void OnDisable()
        {
            GameManager.UnregisterPlayer(name, player.playerRole.role);
        }
    }
}