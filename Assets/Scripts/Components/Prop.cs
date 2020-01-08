using UnityEngine;

namespace Scripts.Components
{
    [CreateAssetMenu(fileName = "New Prop", menuName = "Prop")]
    public sealed class Prop : ScriptableObject
    {
        public float speedMultiplier;

        public float jumpHeightMultiplier;

        public GameObject prefab;
    }
}