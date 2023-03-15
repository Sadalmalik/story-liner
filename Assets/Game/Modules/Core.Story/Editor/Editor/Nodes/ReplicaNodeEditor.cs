using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Self.StoryV2;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ReplicaNode))]
    public class ReplicaNodeEditor : NodeEditor
    {
        private const string REPLICA_NODEVIEW_PATH = "Styles/NodeEditorStyles/ReplicaNodeView";
        private const string REPLICA_STYLE_PATH = "Styles/NodeEditorStyles/ReplicaNodeStyle";

        private SerializedProperty m_CharacterProperty;
        private SerializedProperty m_EmotionProperty;
        private SerializedProperty m_ReplyTextProperty;

        private TextField m_ReplyTextField;




        protected override void OnEnable()
        {
            base.OnEnable();

            m_CharacterProperty = serializedObject.FindProperty(nameof(ReplicaNode.character));
            m_EmotionProperty = serializedObject.FindProperty(nameof(ReplicaNode.emotion));
            m_ReplyTextProperty = serializedObject.FindProperty(nameof(ReplicaNode.localized));
        }

        protected override void CreateNodeGUI(VisualElement nodeGuiRoot)
        {
            var nodeStyleSheet = Resources.Load<StyleSheet>(REPLICA_STYLE_PATH);

            if (!nodeGuiRoot.styleSheets.Contains(nodeStyleSheet))
                nodeGuiRoot.styleSheets.Add(nodeStyleSheet);

            var replicaGuiTemplate = Resources.Load<VisualTreeAsset>(REPLICA_NODEVIEW_PATH);
            var replicaGui = new VisualElement();

            replicaGuiTemplate.CloneTree(replicaGui);

            replicaGui = replicaGui.Q("replica-container");
            nodeGuiRoot.Add(replicaGui);

            CreateCharacterContainer(replicaGui);
            CreateEmotionContainer(replicaGui);

            CreateReplyTextContainer(replicaGui);
        }

        private void CreateCharacterContainer(VisualElement guiRoot)
        {
            var characterContainer = guiRoot.Q<TextField>("character-name");
            characterContainer.RegisterValueChangedCallback(HandleCharacterChanged);
            characterContainer.SetValueWithoutNotify(m_CharacterProperty.stringValue);
        }

        private void CreateEmotionContainer(VisualElement guiRoot)
        {
            var emotionContainer = guiRoot.Q<TextField>("character-emotion");
            emotionContainer.RegisterValueChangedCallback(HandleEmotionChanged);
            emotionContainer.SetValueWithoutNotify(m_EmotionProperty.stringValue);
        }

        private void CreateReplyTextContainer(VisualElement guiRoot)
        {
            m_ReplyTextField = guiRoot.Q<TextField>("character-text");

            m_ReplyTextField.RegisterValueChangedCallback(HandleReplyTextChanged);
            m_ReplyTextField.SetValueWithoutNotify(m_ReplyTextProperty.stringValue);
        }

        private void HandleCharacterChanged(ChangeEvent<string> character)
        {
            m_CharacterProperty.stringValue = character.newValue;
            serializedObject.ApplyModifiedProperties();
        }

        private void HandleEmotionChanged(ChangeEvent<string> emotion)
        {
            m_EmotionProperty.stringValue = emotion.newValue;
            serializedObject.ApplyModifiedProperties();
        }

        private void HandleReplyTextChanged(ChangeEvent<string> replyText)
        {
            m_ReplyTextProperty.stringValue = replyText.newValue;
            serializedObject.ApplyModifiedProperties();
        }
    }
}