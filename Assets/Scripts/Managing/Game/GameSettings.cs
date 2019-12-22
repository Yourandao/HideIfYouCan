using UnityEngine;

namespace Assets.Scripts.Managing.Game
{
    [System.Serializable]
    public class GameSettings
    {
        [Header("Time Settings")]
        public float roundTime;

        public float timeToHide;

        [Header("Roles Settings")]
        [Range(0f, 1f)] public float seekersToHidersRelation;
    }
}