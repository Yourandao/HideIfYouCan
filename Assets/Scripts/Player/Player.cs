using Assets.Scripts.Exceptions;
using Assets.Scripts.PlayerScripts.PlayerRoles;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public sealed class Player : NetworkBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private GameObject firstPersonCamera = default;

        [SerializeField] private GameObject thirdPersonCameraController = default;

        [SerializeField] private GameObject thirdPersonCameraPrefab = default;
        private                  GameObject thirdPersonCameraInstance;

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
            if (isLocalPlayer)
            {
                switch (playerRole.role) {
                    case Roles.Hider:
                        Destroy(thirdPersonCameraInstance);
                        thirdPersonCameraInstance      = Instantiate(thirdPersonCameraPrefab);
                        thirdPersonCameraInstance.name = thirdPersonCameraPrefab.name;

                        firstPersonCamera.SetActive(false);
                        thirdPersonCameraController.SetActive(true);

                        break;
                    case Roles.Seeker:
                        firstPersonCamera.SetActive(true);
                        thirdPersonCameraController.SetActive(false);

                        break;
                    default: throw new UnhandledRoleException(playerRole.role);
                }
            }

            var roleSet = playerRole.GetRoleSet();
            Utility.ToggleComponents(ref roleSet, true);

            Utility.ToggleComponents(ref playerRole.defaultRoleSet, false);
        }
    }
}