using Self.StoryV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Self.Story.Editors
{
    [CustomPropertyDrawer(typeof(Variable))]
    public class VariablePropertyDrawer : PropertyDrawer
    {
        private const BindingFlags c_FieldsFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;




        // it's all drawn inside EditorGuiLayout.BeginHorizontal and EditorGuiLayout.EndHorizontal
        public void DrawProperty(SerializedProperty property, Rect position, SerializedObject serializedObject)
        {
            EditorGUI.BeginChangeCheck();

            var type = property.objectReferenceValue.GetType();
            var fields = type.GetFields(c_FieldsFlags);

            var idField = fields.FirstOrDefault(f => f.Name == "id");

            position.width = (position.width / 5f) - 2f;

            if(idField != null)
            {
                var fieldProperty = serializedObject.FindProperty(idField.Name);

                position = DrawPropertyField(fieldProperty, position);
            }

            foreach (var field in fields)
            {
                if (field.Name == "id")
                    continue;

                var fieldProperty = serializedObject.FindProperty(field.Name);

                // need to change this to Rect position usage

                if (fieldProperty != null)
                {
                    position = DrawPropertyField(fieldProperty, position);
                }
            }

            var propertyType = fields.FirstOrDefault(f => f.Name == "value");

            var missingPropertyPositions = 4 - fields.Count();

            position.x += (missingPropertyPositions * position.width) + (missingPropertyPositions * 2f);

            EditorGUI.LabelField(position, $"{serializedObject.FindProperty(propertyType.Name).propertyType}");

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private Rect DrawPropertyField(SerializedProperty property, Rect position)
        {
            var newPos = position;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    var oldIntValue = property.intValue;
                    var intValue = EditorGUI.IntField(newPos, property.intValue);

                    if (intValue != oldIntValue)
                    {
                        if(property.displayName == property.serializedObject.FindProperty(nameof(IntVariable.value)).displayName)
                        {
                            var minProp = property.serializedObject.FindProperty(nameof(IntVariable.minValue));
                            var maxProp = property.serializedObject.FindProperty(nameof(IntVariable.maxValue));

                            var clampedValue = Mathf.Clamp(intValue, minProp.intValue, maxProp.intValue);

                            property.intValue = clampedValue;
                        }
                        else
                        {
                            property.intValue = intValue;
                        }
                    }

                    break;
                case SerializedPropertyType.Boolean:
                    var oldValue = property.boolValue;

                    var buttonCurrentName = property.boolValue ? "True" : "False";

                    if(GUI.Button(newPos, buttonCurrentName))
                    {
                        property.boolValue = !property.boolValue;
                    }

                    break;
                case SerializedPropertyType.Float:
                    var oldFloatValue = property.floatValue;
                    var floatValue = EditorGUI.FloatField(newPos, property.floatValue);

                    if (!Mathf.Approximately(oldFloatValue, floatValue))
                    {
                        if (property.displayName == property.serializedObject.FindProperty(nameof(FloatVariable.value)).displayName)
                        {
                            var minProp = property.serializedObject.FindProperty(nameof(FloatVariable.minValue));
                            var maxProp = property.serializedObject.FindProperty(nameof(FloatVariable.maxValue));

                            var clampedValue = Mathf.Clamp(floatValue, minProp.floatValue, maxProp.floatValue);

                            property.floatValue = clampedValue;
                        }
                        else
                        {
                            property.floatValue = floatValue;
                        }
                    }

                    break;
                case SerializedPropertyType.String:
                    var oldStringValue = property.stringValue;
                    var newStringValue = EditorGUI.TextField(newPos, property.stringValue);

                    if (newStringValue != oldStringValue)
                        property.stringValue = newStringValue;

                    break;
                default:
                    EditorGUI.LabelField(newPos, $"[{nameof(VariablePropertyDrawer)}.{nameof(DrawPropertyField)}] Not implemented for '{property.propertyType}'");
                    break;
            }

            newPos.x += newPos.width + 2f;

            return newPos;
        }
    }
}