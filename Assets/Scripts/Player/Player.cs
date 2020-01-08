using Assets.Scripts.Player.Settings;

using Mirror;

using Scripts.Components;
using Scripts.Management.Game;
using Scripts.Management.Network;
using Scripts.PlayerScripts.Behaviour;
using Scripts.PlayerScripts.Control;
using Scripts.UI;

using UnityEngine;

namespace Scripts.PlayerScripts
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Transformation))]
    [RequireComponent(typeof(Catching))]
    public sealed class Player : NetworkBehaviour
    {
        [Header("Components")]
        public PlayerController controller;

        public Transformation transformation;

        public Catching catching;

        private UserInterface userInterface;

        [SerializeField] private UnityEngine.Behaviour[] disableOnDeath;

        [HideInInspector] public GameSettings gameSettings;

        private HealthSettings settings;

        [SyncVar] private float currentHealth;

        public bool Paused { get; private set; }

        [Space]
        public LayerMask propMask;

        [HideInInspector]
        [SyncVar] public Role role;

        [HideInInspector]
        [SyncVar] public float healthAmount;

        private void Start()
        {
            controller.player     = this;
            transformation.player = this;
            catching.player       = this;

            gameSettings = ServerManager.Singleton.gameManager.gameSettings;
            settings     = gameSettings.healthSettings;

            currentHealth = settings.maxHealthAmount;
        }

        public void Setup(UserInterface userInterface)
        {
            this.userInterface = userInterface;

            this.userInterface.player = this;
            this.userInterface.UpdateStats();

            ServerManager.Singleton.ToggleSceneCamera(false);

            CmdSetup();
        }

        [Command]
        private void CmdSetup() => RpcSetup();

        [ClientRpc]
        private void RpcSetup()
        {
            if (isLocalPlayer)
            {
                if (role != Role.Hider)
                    transformation.enabled = false;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Paused = !Paused;

                userInterface.TogglePause();
                controller.SetStop(Paused);
            }
        }

        [ClientRpc]
        public void RpcStartGame()
        {
            ServerManager.Singleton.ToggleSceneCamera(false);

            controller.freezed = false;
        }

        [ClientRpc]
        public void RpcStopGame()
        {
            controller.SetStop(true);
        }

        [ClientRpc]
        public void RpcTakeDamage(float amount, uint source)
        {
            if (role != Role.Hider)
                return;

            healthAmount -= amount;

            if (healthAmount <= 0f)
                Die(source);
        }

        private void Die(uint source)
        {
            Utility.ToggleComponents(ref disableOnDeath, false);

            // TODO: Let player choose role (Seeker/Spectator)
        }

        [Command]
        public void CmdBecomeSeeker() => RpcBecomeSeeker();

        [Command]
        public void CmdBecomeSpectator() => RpcBecomeSpectator();

        [ClientRpc]
        private void RpcBecomeSeeker()
        {
            role = Role.Seeker;

            if (isLocalPlayer)
            {
                transformation.enabled = false;

                // TODO: Enable catch
            }
        }

        [ClientRpc]
        private void RpcBecomeSpectator() { }
    }
}