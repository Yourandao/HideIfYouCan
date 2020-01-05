using Scripts.Attributes;

using UnityEditor;

using UnityEngine;

namespace Scripts.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public sealed class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var scene = EditorGUI.ObjectField(position, label,
                                                  GetSceneObject(property.stringValue),
                                                  typeof(SceneAsset), true);

                if (scene is null)
                    property.stringValue = string.Empty;
                else if (scene.name != property.stringValue)
                    if (GetSceneObject(scene.name) is null)
                        Debug.LogWarning($"Scene {scene.name} cannot be used. Add it in build settings before.");
                    else
                        property.stringValue = scene.name;
            }
            else
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
        }

        private static SceneAsset GetSceneObject(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            foreach (var editorScene in EditorBuildSettings.scenes)
                if (editorScene.path.Contains(name))
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);

            Debug.LogError($"Cannot find scene with name {name}");

            return null;
        }
    }
}