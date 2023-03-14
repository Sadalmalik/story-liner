using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.StoryV2
{
    [CustomEditor(typeof(Node))]
    public class NodeEditor : Editor
    {
        private SerializedProperty m_NodeActionsProperty;
        private VisualElement m_NodeActionsContainer;



        protected virtual void OnEnable()
        {
            m_NodeActionsProperty = serializedObject.FindProperty(nameof(Node.behaviours));
        }

        protected virtual VisualElement CreateNodeGUI()
        {
            return new Label($"Implement NodeEditor.CreateNodeGUI for {serializedObject.targetObject.GetType().Name}");
        }

        // basically use this 
        // to display node actions
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.Add(CreateNodeGUI());
            container.Add(CreateButtonsContainer());
            container.Add(CreateNodeActionsContainer());
            UpdateNodeActionsContainer();

            return container;
        }

        private Button CreateButton(string text)
        {
            var button = new Button();
            button.text = text;

            return button;
        }

        private VisualElement CreateButtonsContainer()
        {
            var buttonsContainer = new VisualElement();
            buttonsContainer.name = "buttons-container";
            var buttonsContainerStyle = buttonsContainer.style;
            buttonsContainerStyle.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            buttonsContainerStyle.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            buttonsContainerStyle.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);

            var addButton = CreateButton("Add Action");
            addButton.name = "add-button";
            addButton.clicked += HandleAddButton;

            var buttonStyle = addButton.style;
            buttonStyle.width = new StyleLength(new Length(49f, LengthUnit.Percent));
            buttonStyle.height = new StyleLength(25f);

            var removeButton = CreateButton("Remove Action");
            removeButton.name = "remove-button";
            removeButton.clicked += HandleRemoveButton;

            buttonStyle = removeButton.style;
            buttonStyle.width = new StyleLength(new Length(49f, LengthUnit.Percent));
            buttonStyle.height = new StyleLength(25f);

            buttonsContainer.Add(addButton);
            buttonsContainer.Add(removeButton);

            return buttonsContainer;
        }

        private VisualElement CreateNodeActionsContainer()
        {
            var container = new VisualElement();
            container.name = "node-actions";
            m_NodeActionsContainer = container;

            return container;
        }

        private VisualElement CreateNodeActionContainer(SerializedProperty actionProperty)
        {
            var editor = Editor.CreateEditor(actionProperty.objectReferenceValue);

            var imguiContainer = new IMGUIContainer(() =>
            {
                if (actionProperty.objectReferenceValue != null)
                    editor.OnInspectorGUI();
            });

            return imguiContainer;

            var propertyField = new PropertyField(actionProperty);
            propertyField.BindProperty(serializedObject);

            return propertyField;
        }

        private void HandleAddButton()
        {
            // maybe show a drop down menu right away

            var arraySize = m_NodeActionsProperty.arraySize;

            m_NodeActionsProperty.InsertArrayElementAtIndex(arraySize);

            var newActionProperty = m_NodeActionsProperty.GetArrayElementAtIndex(arraySize);
            var newNodeAction = ScriptableObject.CreateInstance(typeof(SetVariableAction));
            newNodeAction.name = Story.Editors.StoryEditorWindow.GetNodeActionName(serializedObject.targetObject as Node, typeof(SetVariableAction), arraySize);

            AssetDatabase.AddObjectToAsset(newNodeAction, serializedObject.targetObject);
            AssetDatabase.SaveAssets();

            newActionProperty.objectReferenceValue = newNodeAction;

            serializedObject.ApplyModifiedProperties();

            UpdateNodeActionsContainer();
        }

        private void HandleRemoveButton()
        {
            var arraySize = m_NodeActionsProperty.arraySize;

            if(arraySize > 0)
            {
                var objectToRemove = m_NodeActionsProperty.GetArrayElementAtIndex(arraySize - 1);

                if(objectToRemove.objectReferenceValue != null)
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
        }
    }
}