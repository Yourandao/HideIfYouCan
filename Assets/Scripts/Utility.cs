using UnityEngine;

namespace Assets.Scripts
{
    public static class Utility
    {
        public static void ToggleComponents(ref Behaviour[] components, bool state)
        {
            foreach (var component in components)
            {
                component.enabled = state;
            }
        }

        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null)
                return;

            obj.layer = layer;

            foreach (Transform child in obj.transform)
                if (child != null)
                    SetLayerRecursively(child.gameObject, layer);
        }

        public static int LayerMaskToLayer(LayerMask mask) =>
            Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    }
}