using UnityEngine;

namespace Scripts.Props
{
    public sealed class PropHolder : MonoBehaviour
    {
        public Prop prop;

        [SerializeField] private LayerMask propLayer;

        private void Start()
        {
            int layer = Utility.LayerMaskToLayer(propLayer);

            if (gameObject.layer != layer)
                gameObject.layer = layer;
        }
    }
}