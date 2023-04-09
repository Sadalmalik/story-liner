using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ChapterNode))]
    [CustomEditor(typeof(ChapterNode))]
    public class ChapterNodeEditor : NodeEditor
    {
        private SerializedProperty m_ChapterProperty;
        private PropertyField m_ChapterPropertyField;
        private Chapter m_CurrentChapter;



        protected override void CreateNodeGUI(VisualElement root)
        {
            // TODO: [Andrei]
            // maybe display some info about containing nodes here?
            CreateChapterGui(root);
        }

        private void CreateChapterGui(VisualElement root)
        {
            m_ChapterProperty = serializedObject.FindProperty(nameof(ChapterNode.chapter));
            m_ChapterPropertyField = new PropertyField(m_ChapterProperty);

            m_ChapterPropertyField.BindProperty(serializedObject);
            m_ChapterPropertyField.RegisterValueChangeCallback(HandleChapterChanged);

            m_CurrentChapter = m_ChapterProperty.objectReferenceValue as Chapter;

            var editButton = new Button();
            editButton.text = "Edit";
            editButton.clicked += HandleEditButtonClicked;

            root.Add(editButton);
            root.Add(m_ChapterPropertyField);
        }

        private void HandleChapterChanged(SerializedPropertyChangeEvent evt)
        {
            var newValue = evt.changedProperty.objectReferenceValue as Chapter;

            if (newValue == null)
            {
                m_CurrentChapter.parentChapter = null;
                m_CurrentChapter = null;

                return;
            }

            if(CheckParentRecursive(m_NodeView.CurrentChapter, newValue))
            {
                m_ChapterProperty.objectReferenceValue = m_CurrentChapter;
                serializedObject.ApplyModifiedProperties();

                return;
            }

            newValue.parentChapter = m_NodeView.CurrentChapter;
            serializedObject.ApplyModifiedProperties();

            m_CurrentChapter = newValue;
        }

        private bool CheckParentRecursive(Chapter chapter, Chapter targetChapter)
        {
            if (chapter == null)
                return false;

            if (chapter.parentChapter == targetChapter || m_NodeView.CurrentChapter == targetChapter)
                return true;

            return CheckParentRecursive(chapter.parentChapter, targetChapter);
        }

        private void HandleEditButtonClicked()
        {
            if (m_CurrentChapter == null)
                return;

            StoryEditorWindow.Instance.EditorView.Create(m_CurrentChapter as Chapter);
        }
    }
}