using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferencePropertyDrawer : PropertyDrawer
	{
		private Dictionary<string, Variable> m_Variables;
		private SerializedProperty           m_Property;


		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			m_Property = property;

			var chapterPath = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
			var chapter     = (Chapter) AssetDatabase.LoadAssetAtPath(chapterPath, typeof(Chapter));

			m_Variables = new Dictionary<string, Variable>();

			foreach (var variable in chapter.variables.variables)
			{
				m_Variables.Add(variable.id, variable);
			}

			var variableDropDown = new DropdownField();
			variableDropDown.label   = property.displayName;
			variableDropDown.name    = "variable-dropdown";
			variableDropDown.choices = chapter.variables.variables.Select(v => v.id).ToList();

			if (property.managedReferenceValue != null)
			{
				var variableReference = property.managedReferenceValue as VariableReference;

				variableDropDown.SetValueWithoutNotify(variableReference.variable.id);
			}

			variableDropDown.RegisterValueChangedCallback(HandleVariableSelected);

			return variableDropDown;
		}

		private void HandleVariableSelected(ChangeEvent<string> variable)
		{
			if (m_Variables.TryGetValue(variable.newValue, out var selectedVariable))
			{
				m_Property.serializedObject.Update();

				m_Property.managedReferenceValue = new VariableReference() {variable = selectedVariable};

				m_Property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}