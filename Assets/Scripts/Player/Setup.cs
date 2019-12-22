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

        [SerializeField] private GameObject UIPrefab = default;
        private                  GameObject UIInstance;

        [SerializeField] private Behaviour[] componentsToDisable;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToDisable, true);

            UIInstance      = Instantiate(UIPrefab);
            UIInstance.name = UIPrefab.name;

            player.Setup();
            CmdSetName(playerName);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            playerName = GetComponent<NetworkIdentity>().netId.ToString();

            GameManager.RegisterPlayer(playerName, player);
        }

        public void OnDisable()
        {
            Destroy(UIInstance);

            GameManager.UnregisterPlayer(playerName);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;
    }
}