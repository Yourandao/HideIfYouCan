using System.Linq;

using Mirror;

using Scripts.Exceptions;
using Scripts.Management.Network;
using Scripts.PlayerScripts.Control;
using Scripts.PlayerScripts.PlayerBehaviour;
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

        [Header("Layers")]
        public LayerMask firstPersonModelMask;

        public LayerMask hiderMask;

        [HideInInspector]
        [SyncVar] public Role role;

        private Camera[] observableCameras;
        private int      spectatingIndex;

        private GameObject observableModel;

        [Header("FX")]
        [SerializeField] private GameObject deathEffect;

        [SerializeField] private float deathEffectDuration = 3f;

        public bool Paused { get; private set; }

        private void Start()
        {
            if (isServer)
                currentHealth = maxHealthAmount;

            transformation.Setup(this);
            catching.Setup(this);
        }

        public void Setup(UserInterface userInterface, Animator animator)
        {
            this.userInterface        = userInterface;
            this.userInterface.player = this;

            this.userInterface.UpdateRole(role);

            controller.Setup(animator);
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if (role == Role.Spectator && observableCameras != null)
            {
                var desiredCamera = observableCameras[spectatingIndex];

                if (!desiredCamera.enabled)
                {
                    observableModel = desiredCamera.GetComponentInParent<Transformation>().modelHolder.gameObject;

                    Utility.SetLayerRecursively(observableModel, Utility.LayerMaskToLayer(firstPersonModelMask));

                    desiredCamera.enabled = true;
                }

                if (Input.GetButtonDown("Attack"))
                {
                    observableCameras[spectatingIndex].enabled = false;

                    Utility.SetLayerRecursively(observableModel, LayerMask.NameToLayer("Default"));

                    spectatingIndex = ++spectatingIndex % observableCameras.Length;
                }
            }

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

        private void Die(uint source)
        {
            role = Role.Spectator;

            RpcOnDeath();

            TargetBecomeSpectator(connectionToClient);

            // TODO: Show source in kill feed
        }

        [ClientRpc]
        private void RpcOnDeath()
        {
            transformation.modelHolder.gameObject.SetActive(false);

            Destroy(Instantiate(deathEffect, transform.position, transform.rotation), deathEffectDuration);

            if (isLocalPlayer)
                Utility.ToggleComponents(ref disableOnDeath, false);
        }

        [TargetRpc]
        private void TargetBecomeSpectator(NetworkConnection connection)
        {
            observableCameras = ServerManager.GetAllCameras().ToArray();
        }
    }
}