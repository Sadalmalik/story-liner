using System;
using Self.Story;
using Self.Story.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InspectedType(typeof(ActiveNode))]
[CustomEditor(typeof(ActiveNode))]
public class ActiveNodeEditor : NodeEditor
{
	public const string TargetStartNodeEditorKey = "starting-node-editor";
	public const string TargetChapterEditorKey = "starting-chapter-editor";

	private const string BUTTONS_ROOT_NAME = "buttons-container";
	private const string ACTIONS_CONTAINER_NAME = "node-actions-container";
	private const string ACTIONS_CONTAINER_PATH = "Styles/NodeEditorStyles/Arrays/ArrayContainerView";
	private const string ACTIONS_STYLE_PATH = "Styles/NodeEditorStyles/Arrays/ArrayContainerStyle";

	private SerializedProperty m_NodeActionsProperty;

	private VisualElement m_NodeActionsContainer;
	private VisualElement m_NodeActionsEmptyContainer;



	protected override void OnEnable()
	{
		base.OnEnable();

		m_NodeActionsProperty = serializedObject.FindProperty(nameof(ActiveNode.actions));
	}

	protected override void CreateNodeGUI(VisualElement root)
	{
		CreatePlayButton(root);
		CreateActionsContainerGUI(root);
	}

	protected void CreatePlayButton(VisualElement root)
	{
		var playButton = new Button(HandlePlayButtonClicked);
		playButton.name = "play-button";
		playButton.text = "Play From This Node";

		root.Add(playButton);
	}

	protected void CreateActionsContainerGUI(VisualElement root)
	{
		var nodeActionsContainer = CreateContainerFromTemplate(ACTIONS_CONTAINER_PATH, "array-container");
		TryAddStyleSheet(ACTIONS_STYLE_PATH);

		m_NodeActionsContainer = nodeActionsContainer.Q("data-container");
		m_NodeActionsEmptyContainer = nodeActionsContainer.Q("empty-container");

		var buttonsRoot = nodeActionsContainer.Q("buttons-container");
		RegisterButtonEvents(buttonsRoot);

		var label = new Label("Actions");
		var style = label.style;
		style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);

		root.Add(label);
		root.Add(nodeActionsContainer);
		UpdateNodeActionsContainer();
	}

	private void RegisterButtonEvents(VisualElement buttonsRoot)
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
	}

	private void UpdateNodeActionsContainer()
	{
		m_NodeActionsContainer.Clear();

		var arraySize = m_NodeActionsProperty.arraySize;

		for (int i = 0; i < arraySize; i++)
		{
			var nodeAction = m_NodeActionsProperty.GetArrayElementAtIndex(i);
			m_NodeActionsContainer.Add(CreateNodeActionContainer(nodeAction));

			if (i != arraySize - 1)
			{
				var separator = new VisualElement();
				separator.name = "separator";
				m_NodeActionsContainer.Add(separator);
			}
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

	private void HandleAddButton()
	{
		var actions = TypeCache.GetTypesDerivedFrom(typeof(BaseAction));

		// maybe show a drop down menu right away
		var dropDownMenu = new GenericMenu();

		foreach (var action in actions)
		{
			var label = new GUIContent(action.Name);
			var isOn = false;
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
		var arraySize = m_NodeActionsProperty.arraySize;

		m_NodeActionsProperty.InsertArrayElementAtIndex(arraySize);

		var newActionProperty = m_NodeActionsProperty.GetArrayElementAtIndex(arraySize);
		var newNodeAction = ScriptableObject.CreateInstance(resultType);
		newNodeAction.name = StoryEditorWindow.GetNodeActionName(serializedObject.targetObject as BaseNode,
			resultType, arraySize);

		AssetDatabase.AddObjectToAsset(newNodeAction, serializedObject.targetObject);
		AssetDatabase.SaveAssets();

		newActionProperty.objectReferenceValue = newNodeAction;

		serializedObject.ApplyModifiedProperties();

		UpdateNodeActionsContainer();
	}

	private void HandlePlayButtonClicked()
	{
		var targetNodeId = m_NodeView.Node.id;
		var chapter = m_NodeView.CurrentChapter;
		var chapterPath = AssetDatabase.GetAssetPath(chapter);

		EditorPrefs.SetString(TargetStartNodeEditorKey, targetNodeId);
		EditorPrefs.SetString(TargetChapterEditorKey, chapterPath);

		EditorApplication.EnterPlaymode();
	}
}
