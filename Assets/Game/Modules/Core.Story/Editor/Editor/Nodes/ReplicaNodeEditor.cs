using Self.Story.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.StoryV2
{
    [InspectedType(typeof(ReplicaNode))]
    [CustomEditor(typeof(ReplicaNode))]
    public class ReplicaNodeEditor : NodeEditor
    {
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

        protected override VisualElement CreateNodeGUI()
        {
            var container = new VisualElement();

            var upperContainer = new VisualElement();
            var style = upperContainer.style;
            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);

            var lowerContainer = new VisualElement();

            container.Add(upperContainer);
            container.Add(lowerContainer);

            upperContainer.Add(CreateCharacterContainer());
            upperContainer.Add(CreateEmotionContainer());

            lowerContainer.Add(CreateReplyTextContainer());

            return container;
        }

        private VisualElement CreateCharacterContainer()
        {
            var characterContainer = new TextField();
            characterContainer.name = "character-container";
            characterContainer.RegisterValueChangedCallback(HandleCharacterChanged);
            characterContainer.SetValueWithoutNotify(m_CharacterProperty.stringValue);

            return characterContainer;
        }

        private VisualElement CreateEmotionContainer()
        {
            var emotionContainer = new TextField();
            emotionContainer.name = "emotion-container";
            emotionContainer.RegisterValueChangedCallback(HandleEmotionChanged);
            emotionContainer.SetValueWithoutNotify(m_EmotionProperty.stringValue);

            return emotionContainer;
        }

        private VisualElement CreateReplyTextContainer()
        {
            m_ReplyTextField = new TextField();
            m_ReplyTextField.multiline = true;

            m_ReplyTextField.RegisterValueChangedCallback(HandleReplyTextChanged);
            m_ReplyTextField.SetValueWithoutNotify(m_ReplyTextProperty.stringValue);

            var st = m_ReplyTextField.style;
            st.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
            st.width = new StyleLength(new Length(350f, LengthUnit.Pixel));

            return m_ReplyTextField;
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

            var textLength = replyText.newValue.Length;

            var linesAmount = Mathf.Max(1, textLength / 25);

            var st = m_ReplyTextField.style;
            st.height = new StyleLength(linesAmount * 20f);
        }
    }
}