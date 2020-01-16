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
            if (isServer)
                currentHealth = maxHealthAmount;

            transformation.Setup();
            catching.Setup();
        }

        public void Setup(UserInterface userInterface)
        {
            this.userInterface        = userInterface;
            this.userInterface.player = this;

            this.userInterface.UpdateRole(role);

            controller.Setup();
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
            controller.SetFreeze(false, false);

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

        [Server]
        public void TakeDamage(float amount, uint source)
        {
            if (role != Role.Hider)
                return;

            currentHealth -= amount;

            TargetOnDamageTaken(connectionToClient);

            if (currentHealth <= 0f)
                Die(source);
        }

        [TargetRpc]
        private void TargetOnDamageTaken(NetworkConnection connection)
        {
            // TODO: Maybe some effects...
        }

        [Server]
        private void Die(uint source)
        {
            TargetOnDeath(connectionToClient);

            BecomeSpectator();

            // TODO: Show source in kill feed

            Debug.Log("becoming a spectator");
        }

        [TargetRpc]
        private void TargetOnDeath(NetworkConnection connection)
        {
            // TODO: Some local player actions
        }

        [Server]
        private void BecomeSpectator() => TargetBecomeSpectator(connectionToClient);

        [TargetRpc]
        private void TargetBecomeSpectator(NetworkConnection connection)
        {
            // TODO: Spectate for alive players

            controller.enabled = false;

            Debug.Log("became a spectator");
        }
    }
}