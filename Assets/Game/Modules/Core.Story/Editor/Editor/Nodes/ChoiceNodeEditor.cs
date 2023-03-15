using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Self.StoryV2;
using UnityEditor.Experimental.GraphView;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ChoiceNode))]
    public class ChoiceNodeEditor : ReplicaNodeEditor
    {
        private const string CHOICES_NODEVIEW_TEMPLATE_PATH = "Styles/NodeEditorStyles/ChoiceNodeView";
        private const string CHOICE_CONTAINER_TEMPLATE_PATH = "Styles/NodeEditorStyles/ChoiceContainerView";
        private const string CHOICES_STYLE_PATH = "Styles/NodeEditorStyles/ChoiceNodeStyle";

        private SerializedProperty m_ChoicesProperty;

        private VisualTreeAsset m_ChoiceContainerTemplate;
        private VisualElement m_ChoicesRoot;



        protected override void OnEnable()
        {
            base.OnEnable();

            m_ChoicesProperty = serializedObject.FindProperty(nameof(ChoiceNode.choices));
            m_ChoiceContainerTemplate = Resources.Load<VisualTreeAsset>(CHOICE_CONTAINER_TEMPLATE_PATH);
        }

        protected override void CreateNodeGUI(VisualElement nodeGuiRoot)
        {
            base.CreateNodeGUI(nodeGuiRoot);

            TryAddStyleSheet(CHOICES_STYLE_PATH);

            var mainContainer = CreateContainerFromTemplate(CHOICES_NODEVIEW_TEMPLATE_PATH, "main-container");

            m_ChoicesRoot = mainContainer.Q("choices-container");

            nodeGuiRoot.Add(mainContainer);
            CreateButtons();
            CreateChoicesContainers();
        }

        private void CreateChoicesContainers()
        {
            m_ChoicesRoot.Clear();

            m_NodeView.OutputPorts.Clear();
            m_NodeView.outputContainer.Clear();

            var choicesSize = m_ChoicesProperty.arraySize;

            for (int i = 0; i < choicesSize; i++)
            {
                var choiceContainer = CreateContainerFromTemplate(CHOICE_CONTAINER_TEMPLATE_PATH, "choice-container");

                var port = choiceContainer.Q("port-placeholder");
                var portContainer = port.parent;
                choiceContainer.Remove(port);

                var newPort = m_NodeView.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
                newPort.portColor = Color.cyan;
                newPort.AddToClassList("right-port");

                m_NodeView.OutputPorts.Add(newPort);

                portContainer.Add(newPort);

                m_ChoicesRoot.Add(choiceContainer);
            }
        }

        private void CreateButtons()
        {
            var buttonsRoot = m_Root.Q("buttons-container");

            var addButton = buttonsRoot.Q<Button>("add-button");
            addButton.clicked += HandleAddButton;

            var removeButton = buttonsRoot.Q<Button>("remove-button");
            removeButton.clicked += HandleRemoveButton;
        }

        private void HandleAddButton()
        {
            var arraySize = m_ChoicesProperty.arraySize;

            m_ChoicesProperty.InsertArrayElementAtIndex(arraySize);

            var newActionProperty = m_ChoicesProperty.GetArrayElementAtIndex(arraySize);
            var textProperty = newActionProperty.FindPropertyRelative(nameof(ChoiceNode.Choice.localizedText));
            textProperty.stringValue = string.Empty;

            serializedObject.ApplyModifiedProperties();

            //var newNodeAction = ScriptableObject.CreateInstance(resultType);
            //newNodeAction.name = StoryEditorWindow.GetNodeActionName(serializedObject.targetObject as StoryV2.Node, resultType, arraySize);

            //AssetDatabase.AddObjectToAsset(newNodeAction, serializedObject.targetObject);
            //AssetDatabase.SaveAssets();

            //newActionProperty.objectReferenceValue = newNodeAction;

            //serializedObject.ApplyModifiedProperties();

            //UpdateNodeActionsContainer();
        }

        private void HandleRemoveButton()
        {
            return;

            var arraySize = m_ChoicesProperty.arraySize;

            if (arraySize > 0)
            {
                var objectToRemove = m_ChoicesProperty.GetArrayElementAtIndex(arraySize - 1);

                if (objectToRemove.objectReferenceValue != null)
                    AssetDatabase.RemoveObjectFromAsset(objectToRemove.objectReferenceValue);

                m_ChoicesProperty.DeleteArrayElementAtIndex(arraySize - 1);

                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();

                //UpdateNodeActionsContainer();
            }
        }
    }
}