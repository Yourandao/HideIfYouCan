using Mirror;

using Scripts.Exceptions;
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

        [HideInInspector] public UserInterface userInterface;

        [SerializeField] private UnityEngine.Behaviour[] disableOnDeath;

        [Header("Settings")]
        [SerializeField] private float maxHealthAmount = 100f;

        [SerializeField]
        [Range(.01f, 1f)] private float regenerationSpeed = .1f;

        [SerializeField] private float regenerationDelay = 5f;

        [SyncVar] private float currentHealth;

        public bool Paused { get; private set; }

        [Space]
        public LayerMask propMask;

        [HideInInspector]
        [SyncVar] public Role role;

        private void Start()
        {
            currentHealth = maxHealthAmount;
        }

        public void Setup(UserInterface userInterface)
        {
            this.userInterface        = userInterface;
            this.userInterface.player = this;

            this.userInterface.UpdateRole(role);

            catching.enabled       = false;
            transformation.enabled = false;
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

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
            controller.freezed = false;

            if (isLocalPlayer)
            {
                switch (role)
                {
                    case Role.Seeker:
                        catching.enabled = true;

                        break;
                    case Role.Hider:
                        transformation.enabled = true;

                        break;

                    default: throw new UnhandledRoleException(role);
                }
            }
        }

        [ClientRpc]
        public void RpcStopGame()
        {
            controller.SetStop(true);
            
            // TODO: Change UI with game ending
        }

        [ClientRpc]
        public void RpcTakeDamage(float amount, uint source)
        {
            if (role != Role.Hider)
                return;

            currentHealth -= amount;

            if (currentHealth <= 0f)
                Die(source);
        }

        private void Die(uint source)
        {
            Utility.ToggleComponents(ref disableOnDeath, false);

            CmdBecomeSpectator();

            // TODO: Show source in kill feed
        }

        [Command]
        private void CmdBecomeSpectator() => RpcBecomeSpectator();

        [ClientRpc]
        private void RpcBecomeSpectator() { }
    }
}