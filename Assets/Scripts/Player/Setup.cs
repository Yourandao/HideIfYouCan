using Mirror;

using Scripts.Management.Network;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    [RequireComponent(typeof(Player))]
    public sealed class Setup : NetworkBehaviour
    {
        [SerializeField] private Player player;

        [Header("Components Management")]
        [SerializeField] private Behaviour[] componentsToEnable;

        [SerializeField] private CharacterController controller;

        [SerializeField] private GameObject seekerModel;

        [SerializeField] private LayerMask fpModelLayer;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);
            controller.enabled = true;

            Utility.SetLayerRecursively(seekerModel, Utility.LayerMaskToLayer(fpModelLayer));

            player.Setup();
            CmdSetName(name);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            name = GetComponent<NetworkIdentity>().netId.ToString();

            ServerManager.RegisterPlayer(name, player);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;

        public void OnDisable()
        {
            ServerManager.UnregisterPlayer(name, player.role);
        }
    }
}