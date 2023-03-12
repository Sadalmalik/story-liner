using System;
using UnityEditor;
using UnityEngine;

namespace Self.Story.Editors
{
    [CustomEditor(typeof(Choice))]
    public class ChoiceEditor : Editor
    {
        public static event Action<Choice> OnChoiceAdded;
        public static event Action<Choice> OnChoiceRemoved;
        public static event Action<int, string> OnChoiceChanged;

        private SerializedProperty m_ChoicesProperty;
        private Choice m_ChoiceNodeBehaviour;



        private void OnEnable()
        {
            m_ChoiceNodeBehaviour = serializedObject.targetObject as Choice;
            m_ChoicesProperty = serializedObject.FindProperty(nameof(Choice.choices));
        }

        public override void OnInspectorGUI()
        {
            DisplayChoiceAddRemoveButtons();
            DisplayScriptReference();
            DisplayChoicesArray();
        }

        private void DisplayChoiceAddRemoveButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Add Choice")))
            {
                m_ChoicesProperty.InsertArrayElementAtIndex(m_ChoicesProperty.arraySize);

                serializedObject.ApplyModifiedProperties();

                OnChoiceAdded?.Invoke(m_ChoiceNodeBehaviour);
            }

            if (GUILayout.Button(new GUIContent("Remove Choice")))
            {
                if (m_ChoicesProperty.arraySize > 0)
                {
                    m_ChoicesProperty.DeleteArrayElementAtIndex(m_ChoicesProperty.arraySize - 1);

                    serializedObject.ApplyModifiedProperties();

                    OnChoiceRemoved?.Invoke(m_ChoiceNodeBehaviour);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DisplayScriptReference()
        {
            var label = new GUIContent("Script");
            var targetObject = MonoScript.FromScriptableObject(m_ChoiceNodeBehaviour);
            var allowSceneObjects = false;
            var objectType = typeof(Choice);

            GUI.enabled = false;

            EditorGUILayout.ObjectField(label, targetObject, objectType, allowSceneObjects);

            GUI.enabled = true;
        }

        private void DisplayChoicesArray()
        {
            EditorGUI.BeginChangeCheck();

            var choicesSize = m_ChoicesProperty.arraySize;

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < choicesSize; i++)
            {
                var choiceProperty = m_ChoicesProperty.GetArrayElementAtIndex(i);

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();

                choiceProperty.stringValue = EditorGUILayout.TextField(new GUIContent($"choice_{i}"), choiceProperty.stringValue);

                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    OnChoiceChanged?.Invoke(i, choiceProperty.stringValue);
                }
            }

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}