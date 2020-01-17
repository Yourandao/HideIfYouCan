using Mirror;

using Scripts.Management.Network;
using Scripts.UI;

using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private Behaviour[] componentsToEnable;

        [SerializeField] private GameObject seekerModel;

        public LayerMask firstPersonModelLayer;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);

            Utility.SetLayerRecursively(seekerModel, Utility.LayerMaskToLayer(firstPersonModelLayer));

            UIInstance      = Instantiate(UIPrefab);
            UIInstance.name = UIPrefab.name;

            userInterface = UIInstance.GetComponent<UserInterface>();

            player.Setup(userInterface);

            CmdRegisterPlayer(netId);
        }

        public override void OnStartClient()
        {
	        base.OnStartClient();

            name = netId.ToString();

            if (player.role == Role.Seeker)
				ServerManager.RegisterCamera(player.controller.firstPersonCamera.GetComponent<Camera>());
        }

        private void OnDestroy()
        {
            if (isServer)
                ServerManager.UnregisterPlayer(netId);

            if (player.role == Role.Seeker)
				ServerManager.UnregisterCamera(player.controller.firstPersonCamera.GetComponent<Camera>());

            if (isLocalPlayer)
            {
                Destroy(UIInstance);

                SceneManager.LoadSceneAsync(ServerManager.singleton.RoomScene);
            }
        }

        [Command]
        private void CmdRegisterPlayer(uint id)
        {
            ServerManager.RegisterPlayer(id, player);

            gameObject.name = id.ToString();
        }
    }
}