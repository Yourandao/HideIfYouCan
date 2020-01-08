using Assets.Scripts.Player.Settings;

using Scripts.PlayerScripts.Settings;

using UnityEngine;

namespace Scripts.Management.Game
{
    [System.Serializable]
    public sealed class GameSettings
    {
        [Header("Time Settings")]
        public float maxWaitingTime = 30f;

        public float freezeTime = 10f;

        public float hideTime = 60f;

        public float seekTime = 300f;

        public float endingTime = 30f;

        [Header("Roles Settings")]
        [Range(.1f, 1f)] public float seekersToHidersRelation = .25f;

        public ControllerSettings controllerSettings = new ControllerSettings();

        public HealthSettings healthSettings = new HealthSettings();

        public HiderSettings hiderSettings = new HiderSettings();

        public SeekerSettings seekerSettings = new SeekerSettings();
    }
}