using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Self.StoryV2;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ChoiceNode))]
    [CustomEditor(typeof(ChoiceNode))]
    public class ChoiceNodeEditor : ReplicaNodeEditor
    {
        private const string CHOICES_NODEVIEW_TEMPLATE_PATH = "Styles/NodeEditorStyles/ChoiceNodeView";
        private const string CHOICE_CONTAINER_TEMPLATE_PATH = "Styles/NodeEditorStyles/ChoiceContainerView";
        private const string CHOICES_STYLE_PATH = "Styles/NodeEditorStyles/ChoiceNodeStyle";

        private SerializedProperty m_ChoicesProperty;
        private Dictionary<IEventHandler, SerializedProperty> m_ChoicesPropertyMap;

        private VisualElement m_ChoicesArrayContainer;
        private VisualElement m_ChoicesEmptyArrayContainer;



        protected override void OnEnable()
        {
            base.OnEnable();

            m_ChoicesProperty = serializedObject.FindProperty(nameof(ChoiceNode.choices));
            m_ChoicesPropertyMap = new Dictionary<IEventHandler, SerializedProperty>();
        }

        protected override void CreateNodeGUI(VisualElement nodeGuiRoot)
        {
            base.CreateNodeGUI(nodeGuiRoot);

            TryAddStyleSheet(CHOICES_STYLE_PATH);

            var mainContainer = CreateContainerFromTemplate(CHOICES_NODEVIEW_TEMPLATE_PATH, "main-container");

            m_ChoicesArrayContainer = mainContainer.Q("data-container");
            m_ChoicesEmptyArrayContainer = mainContainer.Q("empty-container");

            nodeGuiRoot.Add(mainContainer);
            CreateButtons();
            CreateChoicesContainers();
        }

        private void CreateChoicesContainers()
        {
            m_ChoicesArrayContainer.Clear();

            m_NodeView.OutputPorts.Clear();
            m_NodeView.outputContainer.Clear();

            var choicesSize = m_ChoicesProperty.arraySize;

            for (int i = 0; i < choicesSize; i++)
            {
                AddPort(i);
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
            serializedObject.Update();

            AddChoiceElement();
            AddNextNodeElement();

            serializedObject.ApplyModifiedProperties();

            AddPort(m_ChoicesProperty.arraySize - 1);

            AssetDatabase.SaveAssets();
        }

        private void HandleRemoveButton()
        {
            if (m_ChoicesProperty.arraySize < 1)
                return;

            serializedObject.Update();

            RemoveChoiceElement();
            RemoveNextNodeElement();

            serializedObject.ApplyModifiedProperties();

            RemovePort();

            AssetDatabase.SaveAssets();
        }

        private void AddChoiceElement()
        {
            var arraySize = m_ChoicesProperty.arraySize;

            m_ChoicesProperty.InsertArrayElementAtIndex(arraySize);

            var newChoiceProperty = m_ChoicesProperty.GetArrayElementAtIndex(arraySize);
            var textProperty = newChoiceProperty.FindPropertyRelative(nameof(ChoiceNode.Choice.localizedText));

            textProperty.stringValue = string.Empty;
        }

        private void RemoveChoiceElement()
        {
            var arraySize = m_ChoicesProperty.arraySize;

            m_ChoicesProperty.DeleteArrayElementAtIndex(arraySize - 1);
        }

        private void AddNextNodeElement()
        {
            var arraySize = m_NextNodesProperty.arraySize;

            m_NextNodesProperty.InsertArrayElementAtIndex(arraySize);

            var newNextNodePropety = m_NextNodesProperty.GetArrayElementAtIndex(arraySize);

            newNextNodePropety.stringValue = string.Empty;
            EditorUtility.SetDirty(m_NextNodesProperty.serializedObject.targetObject);
        }

        private void RemoveNextNodeElement()
        {
            var arraySize = m_NextNodesProperty.arraySize;

            m_NextNodesProperty.DeleteArrayElementAtIndex(arraySize - 1);
            EditorUtility.SetDirty(m_NextNodesProperty.serializedObject.targetObject);
        }

        private void AddPort(int atIndex)
        {
            if(m_ChoicesArrayContainer.ClassListContains("hidden"))
                m_ChoicesArrayContainer.RemoveFromClassList("hidden");

            if(!m_ChoicesEmptyArrayContainer.ClassListContains("hidden"))
                m_ChoicesEmptyArrayContainer.AddToClassList("hidden");

            var choiceContainer = CreateContainerFromTemplate(CHOICE_CONTAINER_TEMPLATE_PATH, "choice-container");
            var choiceProperty = m_ChoicesProperty
                                        .GetArrayElementAtIndex(atIndex)
                                        .FindPropertyRelative(nameof(ChoiceNode.Choice.localizedText));

            var choiceTextContainer = choiceContainer.Q<TextField>("choice-text-template");
            m_ChoicesPropertyMap.Add(choiceTextContainer, choiceProperty);

            choiceTextContainer.label = $"choice-{atIndex + 1}";

            choiceTextContainer.RegisterValueChangedCallback(HandleChoiceTextChanged);
            choiceTextContainer.SetValueWithoutNotify(choiceProperty.stringValue);

            var port = choiceContainer.Q("port-placeholder");
            var portContainer = port.parent;
            choiceContainer.Remove(port);

            var newPort = m_NodeView.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
            newPort.portColor = Color.cyan;
            newPort.AddToClassList("right-port");

            m_NodeView.OutputPorts.Add(newPort);

            portContainer.Add(newPort);

            m_ChoicesArrayContainer.Add(choiceContainer);
        }

        private void RemovePort()
        {
            var portCount = m_NodeView.OutputPorts.Count;
            var lastPort = m_NodeView.OutputPorts[portCount - 1];

            if(portCount == 1)
            {
                if (!m_ChoicesArrayContainer.ClassListContains("hidden"))
                    m_ChoicesArrayContainer.AddToClassList("hidden");

                if (m_ChoicesEmptyArrayContainer.ClassListContains("hidden"))
                    m_ChoicesEmptyArrayContainer.RemoveFromClassList("hidden");
            }

            StoryEditorWindow.Instance.EditorView.DeleteElements(new List<GraphElement> { lastPort });

            var choiceContainer = m_ChoicesArrayContainer.Children().ElementAt(portCount - 1);

            m_ChoicesPropertyMap.Remove(choiceContainer);
            m_ChoicesArrayContainer.RemoveAt(portCount - 1);
            m_NodeView.OutputPorts.RemoveAt(portCount - 1);
        }

        private void HandleChoiceTextChanged(ChangeEvent<string> choiceText)
        {
            var choiceProperty = m_ChoicesPropertyMap[choiceText.target];
            choiceProperty.stringValue = (string)choiceText.newValue;
            serializedObject.ApplyModifiedProperties();
        }
    }
}