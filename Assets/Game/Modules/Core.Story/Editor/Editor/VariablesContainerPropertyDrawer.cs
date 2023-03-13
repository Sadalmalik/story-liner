using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Self.Story.Editors
{
    [CustomPropertyDrawer(typeof(VariablesContainer))]
    public class VariablesContainerPropertyDrawer : PropertyDrawer
    {
        private string serializedFloatVariablesName = nameof(VariablesContainer.serializedFloatVariables);
        private string serializedIntVariablesName = nameof(VariablesContainer.serializedIntVariables);
        private string serializedBoolVariablesName = nameof(VariablesContainer.serializedBoolVariables);
        private string serializedStringVariablesNames = nameof(VariablesContainer.serializedStringVariables);



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // make a list of all the variables
            // draw them one by one like a table
            // [Variable Id] : [Variable Type] : [Variable Value]

            base.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;

            var floatVariablesSize = property.FindPropertyRelative(serializedFloatVariablesName).arraySize;
            var intVariablesSize = property.FindPropertyRelative(serializedIntVariablesName).arraySize;
            var boolVariablesSize = property.FindPropertyRelative(serializedBoolVariablesName).arraySize;
            var stringVariablesSize = property.FindPropertyRelative(serializedStringVariablesNames).arraySize;

            return 20f + (floatVariablesSize * 20f) + (intVariablesSize * 20f) + (boolVariablesSize * 20f) + (stringVariablesSize * 20f); 
        }

        private void DrawAddRemoveButtons(Rect position, SerializedProperty property)
        {

        }
    }
}