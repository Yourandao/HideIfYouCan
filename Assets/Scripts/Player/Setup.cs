using Mirror;

using Scripts.Management.Network;
using Scripts.UI;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    [RequireComponent(typeof(Player))]
    public sealed class Setup : NetworkBehaviour
    {
        [SerializeField] private Player player;

        [SerializeField] private GameObject    UIPrefab;
        private                  GameObject    UIInstance;
        private                  UserInterface userInterface;

        [Header("Components Management")]
        [SerializeField] private UnityEngine.Behaviour[] componentsToEnable;

        [SerializeField] private CharacterController controller;

        [SerializeField] private GameObject seekerModel;

        [SerializeField] private LayerMask firstPersonModelLayer;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);
            controller.enabled = true;

            Utility.SetLayerRecursively(seekerModel, Utility.LayerMaskToLayer(firstPersonModelLayer));

            UIInstance      = Instantiate(UIPrefab);
            UIInstance.name = UIPrefab.name;

            userInterface = UIInstance.GetComponent<UserInterface>();

            player.Setup(userInterface);
            CmdSetName(name);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            uint id = GetComponent<NetworkIdentity>().netId;

            name = id.ToString();

            ServerManager.RegisterPlayer(id, player);
        }

        [Command]
        private void CmdSetName(string name) => gameObject.name = name;

        public void OnDisable()
        {
            ServerManager.UnregisterPlayer(netId, player.role);

            userInterface.UpdateStats();

            if (isLocalPlayer)
                ServerManager.Singleton.ToggleSceneCamera(true);
        }
    }
}