using UnityEngine;

namespace Assets.Scripts.Managing.Game
{
    [System.Serializable]
    public sealed class GameSettings
    {
        [Header("Time Settings")]
        public float roundTime;

        public float timeToHide;

        [Header("Roles Settings")]
        [Range(.1f, 1f)] public float seekersToHidersRelation;
    }
}