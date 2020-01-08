using Mirror;

using Scripts.PlayerScripts.Settings;

using UnityEngine;

namespace Scripts.PlayerScripts.Behaviour
{
    public sealed class Catching : NetworkBehaviour
    {
        [HideInInspector] public Player player;

        private SeekerSettings settings;

        private float fireRateZero;

        private int ammoLeft;

        private void Start()
        {
            settings = player.gameSettings.seekerSettings;

            ammoLeft = settings.magazineCapacity;
        }

        private void Update()
        {
            if (player.Paused)
                return;

            if (Input.GetButtonDown("Attack") && Time.time >= fireRateZero)
            {
                // TODO: Shoot

                fireRateZero = Time.time + 60f / settings.fireRate;
            }

            if (ammoLeft < settings.magazineCapacity &&
                Input.GetButtonDown("Reload")) { }
        }
    }
}