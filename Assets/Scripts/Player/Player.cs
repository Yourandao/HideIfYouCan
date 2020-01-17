using System.Collections.Generic;
using System.Linq;

using Mirror;

using Mono.CecilX.Cil;

using Scripts.Exceptions;
using Scripts.Management.Network;
using Scripts.PlayerScripts.PlayerBehaviour;
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

		[SerializeField] private Behaviour[] disableOnDeath;

		[Header("Settings")]
		[SerializeField] private float maxHealthAmount = 100f;

		[SerializeField]
		[Range(.01f, 1f)] private float regenerationSpeed = .1f;

		[SerializeField] private float regenerationDelay = 5f;

		[SyncVar] private float currentHealth;

		private List<Camera> playerCameras;

		private int spectatingIndex = 0;

		[Range(0.0f, 1.0f)]
		[SerializeField] private float cameraLerpFactor = .25f;

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

			if (role == Role.Spectator && playerCameras != null)
			{
				var currentCamera = playerCameras[spectatingIndex];

				if (!currentCamera.enabled)
				{
					var model = currentCamera.GetComponentInParent<Transformation>().modelHolder.gameObject;
					Utility.SetLayerRecursively(model, Utility.LayerMaskToLayer(currentCamera.GetComponentInParent<Setup>().firstPersonModelLayer));

					currentCamera.enabled = true;
				}

				if (Input.GetButtonDown("Attack"))
				{
					playerCameras[spectatingIndex].enabled = false;
					spectatingIndex = ++spectatingIndex % playerCameras.Count;
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

			Debug.Log(currentHealth);

			TargetOnDamageTaken(connectionToClient);

			if (currentHealth <= 0f)
			{
				Die(source);
				Debug.Log("hui");
			}
		}

		[TargetRpc]
		private void TargetOnDamageTaken(NetworkConnection connection)
		{
			// TODO: Maybe some effects...
		}

		//comment code below

		private void Die(uint source)
		{
			role = Role.Spectator;

			RpcOnDeath();

			TargetBecomeSpectator(connectionToClient);

			// TODO: Show source in kill feed

			Debug.Log("becoming a spectator");
		}

		[ClientRpc]
		private void RpcOnDeath()
		{
			transformation.modelHolder.gameObject.SetActive(false);

			if (isLocalPlayer)
				Utility.ToggleComponents(ref disableOnDeath, false);

			// TODO: Some local player actions
		}

		[TargetRpc]
		private void TargetBecomeSpectator(NetworkConnection connection)
		{
			var cameras = ServerManager.GetAllCameras();

			Debug.Log(cameras);

			// TODO: Spectate for alive players
			playerCameras = cameras;

			Debug.Log("became a spectator");
		}
	}
}