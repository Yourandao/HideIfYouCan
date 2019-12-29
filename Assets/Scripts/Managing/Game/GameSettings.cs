using UnityEngine;

namespace Assets.Scripts.Managing.Game
{
    [System.Serializable]
    public sealed class GameSettings
    {
        [Header("Time Settings")]
        public float freezeTime = 10f;

        public float hideTime = 30f;

        public float seekTime = 90f;

        public float endingTime = 10f;

        [Header("Roles Settings")]
        [Range(.1f, 1f)] public float seekersToHidersRelation = .25f;
    }
}