using UnityEditor;
using UnityEngine;
using PirexGames.Core.UINavigator.Editor;

namespace PirexGames.Core.UINavigator.EditorUI
{
    [CustomEditor(typeof(UIContainer), true)]
    [CanEditMultipleObjects]
    public class UIContainerEditor : UnityEditor.Editor
    {
        private SerializedProperty _nameMode;
        private SerializedProperty _containerName;

        private void OnEnable()
        {
            _nameMode = serializedObject.FindProperty("nameMode");
            _containerName = serializedObject.FindProperty("containerName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nameMode);

            var mode = (ContainerNameMode)_nameMode.enumValueIndex;
            
            if (mode == ContainerNameMode.Default)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(_containerName);
                GUI.enabled = true;
                EditorGUILayout.HelpBox("In Default mode, the container name is forced to 'Default'.", MessageType.Info);
            }
            else
            {
                // Use DelayedTextField so changes only apply on Enter or Unfocus
                string newName = EditorGUILayout.DelayedTextField("Container Name", _containerName.stringValue);
                if (newName != _containerName.stringValue)
                {
                    _containerName.stringValue = newName;
                }
            }

            // Draw remaining properties
            DrawPropertiesExcluding(serializedObject, "m_Script", "nameMode", "containerName");

            if (serializedObject.ApplyModifiedProperties())
            {
                // Auto request regenerate with delay when properties change
                UINavigatorGenerator.RequestGenerate();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Manual Regenerate Hub Accessors"))
            {
                UINavigatorGenerator.Generate();
            }
        }
    }
}
