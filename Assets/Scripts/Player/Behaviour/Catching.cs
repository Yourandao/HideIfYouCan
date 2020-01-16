using System.Collections;

using Mirror;

using Scripts.Management.Network;
using Scripts.PlayerScripts.Control;

using UnityEngine;

namespace Scripts.PlayerScripts.Behaviour
{
    public sealed class Catching : NetworkBehaviour
    {
        private Player player;

        private PlayerController controller;

        [Header("Settings")]
        [SerializeField] private float shotDistance = 25f;

        [SerializeField] private float damage           = 25f;
        [SerializeField] private int   fireRate         = 60;
        [SerializeField] private int   magazineCapacity = 4;
        [SerializeField] private float reloadTime       = 2.5f;

        private float fireRateZero;

        [SyncVar] private int ammoLeft;

        [SyncVar] private bool isReloading;

        private void Start()
        {
            player = GetComponent<Player>();

            controller = player.controller;

            ammoLeft = magazineCapacity;
        }

        private void Update()
        {
            if (player.Paused)
                return;

            if (isReloading)
                return;

            if (Input.GetButton("Attack") && Time.time >= fireRateZero)
            {
                CmdShoot();

                fireRateZero = Time.time + 60f / fireRate;
            }

            if (ammoLeft < magazineCapacity &&
                Input.GetButtonDown("Reload"))
            {
                CmdReload();

                return;
            }

            if (ammoLeft == 0)
                CmdReload();
        }

        private IEnumerator Reloading()
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            ammoLeft = magazineCapacity;

            isReloading = false;
        }

        [Command]
        private void CmdShoot()
        {
            ammoLeft--;

            var from      = controller.firstPersonCamera.position;
            var direction = controller.firstPersonCamera.forward;

            if (Physics.Raycast(from, direction, out var hit, shotDistance, player.propMask))
            {
                uint id = hit.collider.GetComponentInParent<NetworkIdentity>().netId;

                ServerManager.GetPlayer(id).RpcTakeDamage(damage, id);
            }
        }

        [Command]
        private void CmdReload() => StartCoroutine(Reloading());
    }
}