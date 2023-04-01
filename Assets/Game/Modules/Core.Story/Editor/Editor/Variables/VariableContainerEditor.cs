using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Self.Story.Editors
{
	[CustomEditor(typeof(VariablesContainer))]
	public class VariableContainerEditor : Editor
	{
		private SerializedObject         m_ParentObject;
		private SerializedProperty       m_VariablesListProperty;
		private List<SerializedProperty> m_VariablesList;
		private Dictionary<string, Type> m_VariableTypes;

		private VariablePropertyDrawer                                                       m_VariablePropertyDrawer;
		private Dictionary<SerializedProperty, SerializedObject>                             m_SerializedObjects;
		private Dictionary<string, Func<List<SerializedProperty>, List<SerializedProperty>>> m_SortingActions;

		private string m_CurrentSorting;
		private bool   m_SortingDirection;
		private int    m_ElementForDeletion = -1;


		private void OnEnable()
		{
			m_SerializedObjects = new Dictionary<SerializedProperty, SerializedObject>();
			m_SortingActions    = CreateSortingActions();

			var parentObjectPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
			var parentObject     = AssetDatabase.LoadAssetAtPath(parentObjectPath, typeof(Book));

			m_ParentObject           = new SerializedObject(parentObject);
			m_VariablePropertyDrawer = new VariablePropertyDrawer(this);
			m_VariablesListProperty  = serializedObject.FindProperty("_variables"); //nameof(VariablesContainer._variables));
			m_VariablesList          = FillVariablesList(m_VariablesListProperty);

			m_VariableTypes = new Dictionary<string, Type>();

			var variableTypes = TypeCache.GetTypesDerivedFrom<Variable>();

			foreach (var varType in variableTypes)
			{
				m_VariableTypes.Add(varType.FullName, varType);
			}
		}

		private List<SerializedProperty> FillVariablesList(SerializedProperty m_VariablesListProperty)
		{
			var list = new List<SerializedProperty>();

			var variablesCount = m_VariablesListProperty.arraySize;

			for (int i = 0; i < variablesCount; i++)
			{
				list.Add(m_VariablesListProperty.GetArrayElementAtIndex(i));
			}

			return list;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var arraySize = m_VariablesListProperty.arraySize;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical();

			DrawLayout();

			if (arraySize == 0)
			{
				DrawEmptyContainer();
			}
			else
			{
				DrawVariablesArray(arraySize);
			}

			DrawArrayButtons();

			if (m_ElementForDeletion != -1)
				RemoveVariable();

			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				m_ParentObject.ApplyModifiedProperties();
			}
		}

		private void DrawLayout()
		{
			var startPosition = EditorGUILayout.BeginHorizontal();

			startPosition.width = (startPosition.width / 5f) - 2f;

			startPosition = DrawLabelIndented(startPosition, "Id");
			startPosition = DrawLabelIndented(startPosition, "Value");
			startPosition = DrawLabelIndented(startPosition, "MinValue");
			startPosition = DrawLabelIndented(startPosition, "MaxValue");
			DrawLabelIndented(startPosition, "Type");

			// force editor layout to new line
			GUILayout.Label(GUIContent.none);

			EditorGUILayout.EndHorizontal();

			Rect DrawLabelIndented(Rect position, string label)
			{
				if (GUI.Button(position, new GUIContent(label)))
				{
					m_CurrentSorting   = label;
					m_SortingDirection = !m_SortingDirection;

					m_VariablesList = m_SortingActions[m_CurrentSorting].Invoke(m_VariablesList);

					if (m_SortingDirection)
						m_VariablesList.Reverse();
				}

				position.x += position.width + 2f;

				return position;
			}
		}

		private void DrawVariablesArray(int arraySize)
		{
			if (m_CurrentSorting != null)
			{
				var variableCount = m_VariablesList.Count;

				// draw array sorted
				for (int i = 0; i < variableCount; i++)
				{
					DrawVariable(m_VariablesList[i], i);
				}
			}
			else
			{
				// draw array unsorted
				for (int i = 0; i < arraySize; i++)
				{
					var variableElement = m_VariablesListProperty.GetArrayElementAtIndex(i);

					DrawVariable(variableElement, i);
				}
			}
		}

		private void DrawVariable(SerializedProperty variableProperty, int index)
		{
			var startPosition = EditorGUILayout.BeginHorizontal();

			if (!m_SerializedObjects.ContainsKey(variableProperty))
			{
				m_SerializedObjects.Add(variableProperty, new SerializedObject(variableProperty.objectReferenceValue));
			}

			m_VariablePropertyDrawer.DrawProperty(variableProperty, startPosition,
				m_SerializedObjects[variableProperty], index);

			// force editor layout to new line
			GUILayout.Label(GUIContent.none);

			EditorGUILayout.EndHorizontal();
		}

		private void DrawEmptyContainer()
		{
			EditorGUILayout.LabelField(new GUIContent("No Variables Created Yet"));
		}

		private void DrawArrayButtons()
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("Add New Variable")))
			{
				DrawSelectionMenu();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawSelectionMenu()
		{
			var menu = new GenericMenu();

			foreach (var varType in m_VariableTypes)
			{
				menu.AddItem(new GUIContent(varType.Value.Name), false, AddVariable, varType.Value.FullName);
			}

			menu.ShowAsContext();
		}

		private void AddVariable(object typeName)
		{
			var type        = m_VariableTypes[(string) typeName];
			var newVariable = ScriptableObject.CreateInstance(type);
			newVariable.name = $".variables.{type.Name}.some-var";

			AssetDatabase.AddObjectToAsset(newVariable, AssetDatabase.GetAssetPath(serializedObject.targetObject));
			AssetDatabase.SaveAssets();

			var newIndex = m_VariablesListProperty.arraySize;

			m_VariablesListProperty.InsertArrayElementAtIndex(newIndex);

			var newVariableProperty = m_VariablesListProperty.GetArrayElementAtIndex(newIndex);
			newVariableProperty.objectReferenceValue = newVariable;

			serializedObject.ApplyModifiedProperties();
			m_ParentObject.ApplyModifiedProperties();

			m_VariablesList.Add(m_VariablesListProperty.GetArrayElementAtIndex(newIndex));

			AssetDatabase.SaveAssets();
		}

		public void RemoveVariable(int index)
		{
			m_ElementForDeletion = index;
		}

		private void RemoveVariable()
		{
			var variable = m_VariablesListProperty.GetArrayElementAtIndex(m_ElementForDeletion);

			AssetDatabase.RemoveObjectFromAsset(variable.objectReferenceValue);

			m_VariablesListProperty.DeleteArrayElementAtIndex(m_ElementForDeletion);
			m_VariablesList = FillVariablesList(m_VariablesListProperty);

			serializedObject.ApplyModifiedProperties();
			m_ParentObject.ApplyModifiedProperties();

			AssetDatabase.SaveAssets();

			m_ElementForDeletion = -1;
		}

		private Dictionary<string, Func<List<SerializedProperty>, List<SerializedProperty>>> CreateSortingActions()
		{
			var sortingActions = new Dictionary<string, Func<List<SerializedProperty>, List<SerializedProperty>>>();

			sortingActions.Add("Id", (list) => { return SortList(list, new ComparerById()); });
			sortingActions.Add("Value", (list) => { return SortList(list, new ComparerByValue()); });
			sortingActions.Add("MinValue", (list) => { return SortList(list, new ComparerByMinValue()); });
			sortingActions.Add("MaxValue", (list) => { return SortList(list, new ComparerByMaxValue()); });
			sortingActions.Add("Type", (list) => { return SortList(list, new ComparerByType()); });

			return sortingActions;
		}

		private List<SerializedProperty> SortList(List<SerializedProperty> list, IComparer<SerializedProperty> comparer)
		{
			var sortedList = new List<SerializedProperty>();

			if (list.Any(elem => elem.objectReferenceValue is IntVariable))
			{
				var intVariables = list.Where(elem => elem.objectReferenceValue is IntVariable).ToList();

				intVariables.Sort(comparer);
				sortedList.AddRange(intVariables);
			}

			if (list.Any(elem => elem.objectReferenceValue is FloatVariable))
			{
				var floatVariables = list.Where(elem => elem.objectReferenceValue is FloatVariable).ToList();

				floatVariables.Sort(comparer);
				sortedList.AddRange(floatVariables);
			}

			if (list.Any(elem => elem.objectReferenceValue is BoolVariable))
			{
				var boolVariables = list.Where(elem => elem.objectReferenceValue is BoolVariable).ToList();

				boolVariables.Sort(comparer);
				sortedList.AddRange(boolVariables);
			}

			if (list.Any(elem => elem.objectReferenceValue is StringVariable))
			{
				var stringVariables = list.Where(elem => elem.objectReferenceValue is StringVariable).ToList();

				stringVariables.Sort(comparer);
				sortedList.AddRange(stringVariables);
			}

			return sortedList;
		}
	}
}