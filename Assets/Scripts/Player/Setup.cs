using Assets.Scripts.Managing.Game;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    [RequireComponent(typeof(Player))]
    public class Setup : NetworkBehaviour
    {
        [SerializeField] private Player player = default;
        private                  string playerName;

        [SerializeField] private Behaviour[] componentsToEnable;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);
            GetComponent<CharacterController>().enabled = true;

            player.Setup();
            CmdSetName(playerName);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            playerName = GetComponent<NetworkIdentity>().netId.ToString();

            GameManager.Players.Add(playerName, player);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;

        public void OnDisable()
        {
            GameManager.Players.Remove(playerName);
        }
    }
}