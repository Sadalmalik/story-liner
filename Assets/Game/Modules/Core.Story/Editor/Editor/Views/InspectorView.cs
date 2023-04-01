using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Self.Story.Editors
{
	public class InspectorView : VisualElement
	{
		private string BEHAVIOURS_ARRAY_NAME = nameof(Node.behaviours);

		public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
		{
		}

		private NodeView         m_SelectedNodeView;
		private Node             m_NodeData;
		private VisualElement    m_MainBehaviourEditor;
		private SerializedObject m_SerializedNode;
		private string           m_CroppedGuid => m_NodeData.id.Substring(0, 6);
		private ScrollView       m_ScrollView;


		public InspectorView()
		{
		}

		public void UpdateSelection(NodeView selected)
		{
			if (m_ScrollView == null)
				m_ScrollView = this.Q<ScrollView>();
			else
				m_ScrollView.contentContainer.Clear();

			m_NodeData = selected.Node;

			if (m_SelectedNodeView != selected)
				m_SerializedNode = new SerializedObject(m_NodeData);

			m_SelectedNodeView = selected;
		}

#region BEHAVIOURS ARRAY

		private void DisplayBehavioursArray()
		{
			var spacer      = new VisualElement();
			var spacerStyle = spacer.style;
			spacerStyle.height = new StyleLength(10f);

			m_ScrollView.contentContainer.Add(spacer);
			m_ScrollView.contentContainer.Add(new Label("Sub Behaviours"));

			DisplayBehaviourArrayButtons();

			var choices = TypeCache
				.GetTypesDerivedFrom(typeof(NodeAction))
				.Where(t => !t.IsAbstract)
				.Select(t => t.Name)
				.ToList();

			var behavioursArray = m_SerializedNode.FindProperty(BEHAVIOURS_ARRAY_NAME);
			var arraySize       = behavioursArray.arraySize;

			for (int i = 0; i < arraySize; i++)
			{
				var behaviourContainer = DisplayBehaviourContainer();
				var behaviourSelector  = DisplayBehaviourSelector(choices, i);
				var behaviourEditor    = DisplayBehaviourEditor(i);

				behaviourContainer.Add(behaviourSelector);
				behaviourContainer.Add(behaviourEditor);

				m_ScrollView.Add(behaviourContainer);
			}
		}

		private void DisplayBehaviourArrayButtons()
		{
			var addButton = new Button(HandleAddBehaviourButtonClick)
			{
				text = "Add Behaviour"
			};

			var addButtonStyle = addButton.style;
			addButtonStyle.flexGrow = new StyleFloat(1f);

			var removeButton = new Button(HandleRemoveBehaviourButtonClick)
			{
				text = "Remove Behaviour"
			};

			var removeButtonStyle = removeButton.style;
			removeButtonStyle.flexGrow = new StyleFloat(1f);

			var buttonContainer = new VisualElement();
			var containerStyle  = buttonContainer.style;
			containerStyle.flexDirection  = new StyleEnum<FlexDirection>(FlexDirection.Row);
			containerStyle.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);

			buttonContainer.Add(addButton);
			buttonContainer.Add(removeButton);

			m_ScrollView.contentContainer.Add(buttonContainer);
		}

		private void HandleAddBehaviourButtonClick()
		{
			var behavioursArray = m_SerializedNode.FindProperty(BEHAVIOURS_ARRAY_NAME);
			var arraySize       = behavioursArray.arraySize;

			behavioursArray.InsertArrayElementAtIndex(arraySize);

			var newElement = behavioursArray.GetArrayElementAtIndex(arraySize);
			var previousElementTypeName = newElement.objectReferenceValue != null
				? newElement.objectReferenceValue.GetType().Name
				: null;

			newElement.objectReferenceValue = null;

			HandleBehaviourSelected(previousElementTypeName, arraySize);
		}

		private void HandleRemoveBehaviourButtonClick()
		{
			var behavioursArray = m_SerializedNode.FindProperty(BEHAVIOURS_ARRAY_NAME);
			var arraySize       = behavioursArray.arraySize;

			if (arraySize == 0)
				return;

			var elementAtIndex = behavioursArray.GetArrayElementAtIndex(arraySize - 1);

			if (elementAtIndex.objectReferenceValue != null)
			{
				AssetDatabase.RemoveObjectFromAsset(elementAtIndex.objectReferenceValue);
				AssetDatabase.SaveAssets();
			}

			behavioursArray.DeleteArrayElementAtIndex(arraySize - 1);

			m_SerializedNode.ApplyModifiedProperties();
			UpdateSelection(m_SelectedNodeView);
		}

		private VisualElement DisplayBehaviourSelector(List<string> choices, int index)
		{
			Func<string, string> convertedHandler = (choice) => { return HandleBehaviourSelected(choice, index); };

			var choiceIndex = m_NodeData.behaviours[index] != null
				? choices.IndexOf(m_NodeData.behaviours[index].GetType().Name)
				: 0;
			var behaviourSelector = new DropdownField("Behaviour:", choices, choiceIndex, convertedHandler);

			return behaviourSelector;
		}

		private VisualElement DisplayBehaviourEditor(int index)
		{
			var behaviourAtIndex = m_SerializedNode
				.FindProperty(BEHAVIOURS_ARRAY_NAME)
				.GetArrayElementAtIndex(index);

			var behaviourEditor = Editor.CreateEditor(behaviourAtIndex.objectReferenceValue);
			var editorContainer = new IMGUIContainer(() =>
			{
				if (behaviourAtIndex.objectReferenceValue != null)
					behaviourEditor.OnInspectorGUI();
			});

			return editorContainer;
		}

		private string HandleBehaviourSelected(string selectedBehaviourClassName, int arrayIndex)
		{
			var targetBehaviourClassName = selectedBehaviourClassName;

			if (string.IsNullOrEmpty(targetBehaviourClassName))
				targetBehaviourClassName = TypeCache
					.GetTypesDerivedFrom(typeof(NodeAction))
					.Where(t => !t.IsAbstract)
					.ElementAt(0).Name;

			var behaviourProperty = m_SerializedNode.FindProperty(BEHAVIOURS_ARRAY_NAME);
			var behaviourAtIndex  = behaviourProperty.GetArrayElementAtIndex(arrayIndex);

			var selectedBehaviour = TypeCache
				.GetTypesDerivedFrom(typeof(NodeAction))
				.Where(t => !t.IsAbstract)
				.First(t => t.Name == targetBehaviourClassName);

			if (behaviourAtIndex.objectReferenceValue == null
			    || behaviourAtIndex.objectReferenceValue.GetType() != selectedBehaviour)
			{
				var behaviourInstance = NodeAction.CreateInstance(selectedBehaviour) as NodeAction;
				behaviourInstance.name = StoryEditorWindow.GetNodeActionName(m_NodeData, selectedBehaviour, arrayIndex);

				if (behaviourAtIndex.objectReferenceValue != null)
				{
					AssetDatabase.RemoveObjectFromAsset(behaviourAtIndex.objectReferenceValue);
				}

				AssetDatabase.AddObjectToAsset(behaviourInstance, m_NodeData);
				AssetDatabase.SaveAssets();

				behaviourAtIndex.objectReferenceValue = behaviourInstance;
				m_SerializedNode.ApplyModifiedProperties();

				UpdateSelection(m_SelectedNodeView);

				m_SelectedNodeView.UpdateTitle();
			}

			return targetBehaviourClassName;
		}

		private VisualElement DisplayBehaviourContainer()
		{
			var behaviourContainer = new VisualElement();
			var bContainerStyle    = behaviourContainer.style;

			//behaviourContainerStyle.backgroundColor = new StyleColor(new UnityEngine.Color(0.12f, 0.12f, 0.12f, 1.12f));
			bContainerStyle.SetBorderRadius(new StyleLength(3f));
			bContainerStyle.SetBorder(new StyleFloat(2f));
			bContainerStyle.SetBorderColor(new UnityEngine.Color(0.12f, 0.12f, 0.12f, 1.0f));
			bContainerStyle.SetPadding(new StyleLength(3f));
			bContainerStyle.SetMargin(new StyleLength(3f), new StyleLength(0f));

			return behaviourContainer;
		}

		private void DisplaySeparator()
		{
			var separator = new VisualElement();
			var sStyle    = separator.style;
			sStyle.height          = new StyleLength(2f);
			sStyle.marginTop       = sStyle.marginBottom = new StyleLength(2f);
			sStyle.backgroundColor = new StyleColor(new UnityEngine.Color(0.5f, 0.5f, 0.5f, 1f));

			m_ScrollView.contentContainer.Add(separator);
		}

#endregion
	}
}