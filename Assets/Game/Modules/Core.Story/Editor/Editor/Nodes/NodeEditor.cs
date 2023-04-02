using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
	[CustomEditor(typeof(Node), true)]
	public class NodeEditor : Editor
	{
		private const string NODE_GUI_ROOT_NAME     = "node-editor-container";
		private const string BUTTONS_ROOT_NAME      = "buttons-container";
		private const string ACTIONS_CONTAINER_NAME = "node-actions-container";

		protected SerializedProperty m_NextNodesProperty;
		private   SerializedProperty m_NodeActionsProperty;

		private VisualElement m_NodeActionsContainer;
		private VisualElement m_NodeActionsEmptyContainer;

		protected VisualElement m_Root;
		protected VisualElement m_NodeGuiContainer;
		protected NodeView      m_NodeView;


		protected virtual void OnEnable()
		{
			m_NextNodesProperty   = serializedObject.FindProperty(nameof(Node.nextNodes));
			m_NodeActionsProperty = serializedObject.FindProperty(nameof(Node.behaviours));
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = false;

			base.OnInspectorGUI();

			GUI.enabled = true;
		}

		protected virtual void CreateNodeGUI(VisualElement root)
		{
			root.Add(
				new Label($"Implement NodeEditor.CreateNodeGUI for {serializedObject.targetObject.GetType().Name}"));
		}

		// basically use this 
		// to display node actions
		public void CreateInspectorGUI(VisualElement root, NodeView nodeView)
		{
			m_Root     = root;
			m_NodeView = nodeView;

			m_NodeGuiContainer = m_Root.Q(NODE_GUI_ROOT_NAME);
			var buttonsRoot = m_Root.Q(ACTIONS_CONTAINER_NAME).Q(BUTTONS_ROOT_NAME);
			m_NodeActionsContainer      = m_Root.Q(ACTIONS_CONTAINER_NAME).Q("data-container");
			m_NodeActionsEmptyContainer = m_Root.Q(ACTIONS_CONTAINER_NAME).Q("empty-container");

			CreateNodeGUI(m_NodeGuiContainer);
			CreateButtons(buttonsRoot);
			UpdateNodeActionsContainer();
		}

		private void CreateButtons(VisualElement buttonsRoot)
		{
			var addButton = buttonsRoot.Q<Button>("add-button");
			addButton.clicked += HandleAddButton;

			var removeButton = buttonsRoot.Q<Button>("remove-button");
			removeButton.clicked += HandleRemoveButton;
		}

		private VisualElement CreateNodeActionContainer(SerializedProperty actionProperty)
		{
			var propDrawer = new NodeActionPropertyDrawer();

			return propDrawer.CreatePropertyGUI(actionProperty);

			// var editor = Editor.CreateEditor(actionProperty.objectReferenceValue);
			//
			// var imguiContainer = new IMGUIContainer(() =>
			// {
			// 	if (actionProperty.objectReferenceValue != null)
			// 		editor.OnInspectorGUI();
			// });
			//
			// return imguiContainer;
		}

		private void HandleAddButton()
		{
			var actions = TypeCache.GetTypesDerivedFrom(typeof(NodeAction));

			// maybe show a drop down menu right away
			var dropDownMenu = new GenericMenu();

			foreach (var action in actions)
			{
				var label        = new GUIContent(action.Name);
				var isOn         = false;
				var selectedType = action;

				dropDownMenu.AddItem(label, isOn, HandleActionSelected, selectedType);
			}

			dropDownMenu.ShowAsContext();
		}

		private void HandleRemoveButton()
		{
			// update serializedObject in case the node moved
			serializedObject.Update();

			var arraySize = m_NodeActionsProperty.arraySize;

			if (arraySize > 0)
			{
				var objectToRemove = m_NodeActionsProperty.GetArrayElementAtIndex(arraySize - 1);

				if (objectToRemove.objectReferenceValue != null)
					AssetDatabase.RemoveObjectFromAsset(objectToRemove.objectReferenceValue);

				m_NodeActionsProperty.DeleteArrayElementAtIndex(arraySize - 1);

				serializedObject.ApplyModifiedProperties();

				AssetDatabase.SaveAssets();

				UpdateNodeActionsContainer();
			}
		}

		private void UpdateNodeActionsContainer()
		{
			m_NodeActionsContainer.Clear();

			var arraySize = m_NodeActionsProperty.arraySize;

			for (int i = 0; i < arraySize; i++)
			{
				var nodeAction = m_NodeActionsProperty.GetArrayElementAtIndex(i);
				m_NodeActionsContainer.Add(CreateNodeActionContainer(nodeAction));
			}

			if (arraySize > 0)
			{
				if (m_NodeActionsContainer.ClassListContains("hidden"))
					m_NodeActionsContainer.RemoveFromClassList("hidden");

				if (!m_NodeActionsEmptyContainer.ClassListContains("hidden"))
					m_NodeActionsEmptyContainer.AddToClassList("hidden");
			}
			else
			{
				if (!m_NodeActionsContainer.ClassListContains("hidden"))
					m_NodeActionsContainer.AddToClassList("hidden");

				if (m_NodeActionsEmptyContainer.ClassListContains("hidden"))
					m_NodeActionsEmptyContainer.RemoveFromClassList("hidden");
			}
		}

		private void HandleActionSelected(object selectedType)
		{
			// update serializedObject in case the node moved
			serializedObject.Update();

			if (!(selectedType is Type))
			{
				throw new Exception(
					$"[{typeof(NodeEditor).Name}.{nameof(HandleActionSelected)}] {selectedType} is not Type!");
			}

			var resultType = selectedType as Type;
			var arraySize  = m_NodeActionsProperty.arraySize;

			m_NodeActionsProperty.InsertArrayElementAtIndex(arraySize);

			var newActionProperty = m_NodeActionsProperty.GetArrayElementAtIndex(arraySize);
			var newNodeAction     = ScriptableObject.CreateInstance(resultType);
			newNodeAction.name = StoryEditorWindow.GetNodeActionName(serializedObject.targetObject as Node,
				resultType, arraySize);

			AssetDatabase.AddObjectToAsset(newNodeAction, serializedObject.targetObject);
			AssetDatabase.SaveAssets();

			newActionProperty.objectReferenceValue = newNodeAction;

			serializedObject.ApplyModifiedProperties();

			UpdateNodeActionsContainer();
		}

		protected VisualElement CreateContainerFromTemplate(string templatePath, string mainContainerName)
		{
			var template           = Resources.Load<VisualTreeAsset>(templatePath);
			var temporaryContainer = new VisualElement();

			template.CloneTree(temporaryContainer);

			return temporaryContainer.Q(mainContainerName);
		}

		protected void TryAddStyleSheet(string styleSheetPath)
		{
			var nodeStyleSheet = Resources.Load<StyleSheet>(styleSheetPath);

			if (!m_NodeGuiContainer.styleSheets.Contains(nodeStyleSheet))
				m_NodeGuiContainer.styleSheets.Add(nodeStyleSheet);
		}
	}
}