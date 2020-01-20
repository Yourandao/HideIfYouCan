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

        [SerializeField] private Behaviour[] componentsToEnable;

        [SerializeField] private GameObject[] models;
        private                  GameObject   modelInstance;
        private                  Animator     animator;

        // [Attributes.Scene]
        // [SerializeField] private string mainMenuScene;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Utility.ToggleComponents(ref componentsToEnable, true);

            Utility.SetLayerRecursively(modelInstance, Utility.LayerMaskToLayer(player.firstPersonModelMask));

            UIInstance      = Instantiate(UIPrefab);
            UIInstance.name = UIPrefab.name;

            userInterface = UIInstance.GetComponent<UserInterface>();

            player.Setup(userInterface, animator);

            CmdRegisterPlayer(netId);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            name = netId.ToString();

            var model = models[Random.Range(0, models.Length)];

            modelInstance      = Instantiate(model, player.transformation.modelHolder);
            modelInstance.name = model.name;

            animator = modelInstance.GetComponent<Animator>();

            player.transformation.model = modelInstance;
        }

        private void OnDestroy()
        {
            if (isServer)
                ServerManager.UnregisterPlayer(netId);

            if (player.role == Role.Seeker)
                ServerManager.UnregisterCamera(player.controller.firstPersonCamera.GetComponent<Camera>());

            Destroy(modelInstance);

            if (isLocalPlayer)
            {
                Destroy(UIInstance);

                // SceneManager.LoadSceneAsync(mainMenuScene);
            }
        }

        [Command]
        private void CmdRegisterPlayer(uint id) => ServerManager.RegisterPlayer(id, player);
    }
}