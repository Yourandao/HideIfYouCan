using Mirror;

using UnityEngine;

namespace Scripts.Props
{
    public sealed class PropHolder: NetworkBehaviour
    {
        public Prop prop;

        [SerializeField] private LayerMask propLayer;

        private void Start()
        {
            int layer = Utility.LayerMaskToLayer(propLayer);

            if (gameObject.layer != layer)
                Utility.SetLayerRecursively(gameObject, layer);
        }
    }
}