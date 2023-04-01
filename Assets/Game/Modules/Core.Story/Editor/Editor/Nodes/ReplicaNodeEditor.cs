using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Self.StoryV2;
using System.Collections.Generic;
using System.Linq;

namespace Self.Story.Editors
{
	[InspectedType(typeof(ReplicaNode))]
	[CustomEditor(typeof(ReplicaNode))]
	public class ReplicaNodeEditor : NodeEditor
	{
		private const string REPLICA_NODEVIEW_PATH = "Styles/NodeEditorStyles/ReplicaNodeView";
		private const string REPLICA_STYLE_PATH    = "Styles/NodeEditorStyles/ReplicaNodeStyle";

		private SerializedProperty m_CharacterProperty;
		private SerializedProperty m_EmotionProperty;
		private SerializedProperty m_ReplyTextProperty;

		private TextField   m_ReplyTextField;
		private ReplicaNode m_ReplicaNode;

		private DropdownField m_EmotionsField;
		private VisualElement m_CharacterIcon;


		protected override void OnEnable()
		{
			base.OnEnable();

			m_ReplicaNode = serializedObject.targetObject as ReplicaNode;

			m_CharacterProperty = serializedObject.FindProperty(nameof(ReplicaNode.character));
			m_EmotionProperty   = serializedObject.FindProperty(nameof(ReplicaNode.emotion));
			m_ReplyTextProperty = serializedObject.FindProperty(nameof(ReplicaNode.localized));
		}

		protected override void CreateNodeGUI(VisualElement nodeGuiRoot)
		{
			var nodeStyleSheet = Resources.Load<StyleSheet>(REPLICA_STYLE_PATH);

			if (!nodeGuiRoot.styleSheets.Contains(nodeStyleSheet))
				nodeGuiRoot.styleSheets.Add(nodeStyleSheet);

			var replicaGuiTemplate = Resources.Load<VisualTreeAsset>(REPLICA_NODEVIEW_PATH);
			var replicaGui         = new VisualElement();

			replicaGuiTemplate.CloneTree(replicaGui);

			replicaGui = replicaGui.Q("replica-container");
			nodeGuiRoot.Add(replicaGui);

			CreateCharacterContainer(replicaGui);
			CreateEmotionContainer(replicaGui);

			CreateReplyTextContainer(replicaGui);
		}

		private void CreateCharacterContainer(VisualElement guiRoot)
		{
			m_CharacterIcon = guiRoot.Q<VisualElement>("character-name");

			if (m_ReplicaNode.character != null)
			{
				var charStyle = m_CharacterIcon.style;
				var icon      = m_ReplicaNode.character.character.characterIcon;

				charStyle.backgroundImage = new StyleBackground(icon);
			}

			m_CharacterIcon.RegisterCallback<ClickEvent>(HandleCharacterClicked);
		}

		private void CreateEmotionContainer(VisualElement guiRoot)
		{
			m_EmotionsField         = guiRoot.Q<DropdownField>("character-emotion");
			m_EmotionsField.choices = GetCharacterEmotionChoices();

			m_EmotionsField.RegisterValueChangedCallback(HandleEmotionChanged);

			if (m_EmotionProperty.managedReferenceValue != null)
			{
				var emotionRef = (m_EmotionProperty.managedReferenceValue as EmotionReference);
				m_EmotionsField.SetValueWithoutNotify(emotionRef.emotion);
			}
		}

		private void CreateReplyTextContainer(VisualElement guiRoot)
		{
			m_ReplyTextField = guiRoot.Q<TextField>("character-text");

			m_ReplyTextField.RegisterValueChangedCallback(HandleReplyTextChanged);
			m_ReplyTextField.SetValueWithoutNotify(m_ReplyTextProperty.stringValue);
		}

		private List<string> GetCharacterEmotionChoices()
		{
			if (m_ReplicaNode.character == null)
				return new List<string> {"no character selected"};
			else
			{
				return m_ReplicaNode.character.character.emotions.ToList();
			}
		}

		private void HandleCharacterSelected(object selectedCharacter)
		{
			// update in case node has been moved
			serializedObject.Update();

			m_CharacterProperty.managedReferenceValue = new CharacterReference()
				{character = selectedCharacter as Character};

			serializedObject.ApplyModifiedProperties();

			// selected character updated
			// update the emotions list
			serializedObject.Update();

			m_EmotionsField.choices                 = GetCharacterEmotionChoices();
			m_EmotionProperty.managedReferenceValue = new EmotionReference() {emotion = string.Empty};
			m_EmotionsField.SetValueWithoutNotify(string.Empty);

			serializedObject.ApplyModifiedProperties();

			if (m_ReplicaNode.character != null)
			{
				var charStyle = m_CharacterIcon.style;
				var icon      = m_ReplicaNode.character.character.characterIcon;

				charStyle.backgroundImage = new StyleBackground(icon);
			}
		}

		private void HandleEmotionChanged(ChangeEvent<string> selectedEmotion)
		{
			// update in case node has been moved
			serializedObject.Update();
			m_EmotionProperty.managedReferenceValue = new EmotionReference() {emotion = selectedEmotion.newValue};
			serializedObject.ApplyModifiedProperties();
		}

		private void HandleReplyTextChanged(ChangeEvent<string> replyText)
		{
			// update in case node has been moved
			serializedObject.Update();
			m_ReplyTextProperty.stringValue = replyText.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		private void HandleCharacterClicked(ClickEvent evt)
		{
			var currentCharacter = m_CharacterProperty.managedReferenceValue as CharacterReference;
			var selectorMenu     = new GenericMenu();

			foreach (var character in m_NodeView.CurrentChapter.characters)
			{
				var name       = character.characterName;
				var isSelected = currentCharacter != null && currentCharacter.character.characterName == name;
				selectorMenu.AddItem(new GUIContent(name), isSelected, HandleCharacterSelected, character);
			}

			selectorMenu.ShowAsContext();
		}
	}
}