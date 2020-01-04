using UnityEngine;

namespace Scripts.Components
{
    [CreateAssetMenu(fileName = "New Prop", menuName = "Prop")]
    public sealed class Prop : ScriptableObject
    {
        public float speedMultiplier;

        public float jumpForceMultiplier;

        public GameObject prefab;
    }
}