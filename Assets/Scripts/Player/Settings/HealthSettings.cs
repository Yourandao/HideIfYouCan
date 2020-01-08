using UnityEngine;

namespace Assets.Scripts.Player.Settings
{
    [System.Serializable]
    public sealed class HealthSettings
    {
        public                   float maxHealthAmount   = 100f;
        [Range(.01f, 1f)] public float regenerationSpeed = .1f;
        public                   float regenerationDelay = 5f;
    }
}