using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public event Action<NodeView> OnNodeSelected;
        public event Action<NodeView, int> OnNodePortDisconnected;

        public Node Node { get; private set; }
        public string Guid { get; private set; }

        public Port InputPort;
        public List<Port> OutputPorts = new List<Port>();

        private VisualElement m_VariablesContainer;



        public NodeView(Node node) : base("Assets/Game/Modules/Core.Story/StoryEditor/Styles/StoryNodeView.uxml")
        {
            this.Node = node;
            this.viewDataKey = node.id;
            this.Guid = node.id;

            style.left = node.position.x;
            style.top = node.position.y;

            m_VariablesContainer = this.Q<VisualElement>("variables-container");

            var debugInfo = this.Q<Label>("debug-info");
            debugInfo.text = $"id:{node.id}";

            CreateVariableContainer();

            CreateInputPorts();
            CreateOutputPorts();
            SetupStyleClasses();
            UpdateTitle();
        }

        public void UpdateMainBehaviour(NodeBehaviour newBehaviour)
        {
            for (int i = 0; i < OutputPorts.Count; i++)
            {
                OnNodePortDisconnected?.Invoke(this, i);
            }

            if (newBehaviour is Choice choice)
            {
                ChoiceEditor.OnChoiceAdded -= HandleChoiceAdded;
                ChoiceEditor.OnChoiceAdded += HandleChoiceAdded;

                ChoiceEditor.OnChoiceChanged -= HandleChoiceChanged;
                ChoiceEditor.OnChoiceChanged += HandleChoiceChanged;

                ChoiceEditor.OnChoiceRemoved -= HandleChoiceRemoved;
                ChoiceEditor.OnChoiceRemoved += HandleChoiceRemoved;

                if (Node.nextNodes.Count != 1)
                {
                    Node.nextNodes = new List<string>()
                    {
                        string.Empty
                    };
                }
            }

            if(newBehaviour is Replica)
            {
                if (Node.nextNodes.Count != 1)
                {
                    Node.nextNodes = new List<string>()
                    {
                        string.Empty
                    };
                }
            }

            if (newBehaviour is ConditionBehaviour condition)
            {
                if (Node.nextNodes.Count != 2)
                {
                    Node.nextNodes = new List<string>
                    {
                        string.Empty,
                        string.Empty
                    };
                }
            }

            CreateOutputPorts();
            CreateVariableContainer();
        }

        public void UpdatePortName(int index, string name)
        {
            OutputPorts[index].portName = name;
        }

        private void HandleChoiceAdded(Choice choiceBehaviour)
        {
            if (Node.mainBehaviour != choiceBehaviour)
                return;

            Node.nextNodes.Add(string.Empty);

            var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);

            outputPort.portName = "out";
            outputPort.portColor = Color.cyan;

            OutputPorts.Add(outputPort);
            outputContainer.Add(outputPort);
        }

        private void HandleChoiceRemoved(Choice choiceBehaviour)
        {
            if (Node.mainBehaviour != choiceBehaviour)
                return;

            Node.nextNodes.RemoveAt(Node.nextNodes.Count - 1);
            var port = OutputPorts[choiceBehaviour.choices.Count];

            OnNodePortDisconnected?.Invoke(this, choiceBehaviour.choices.Count);

            if(port != null)
            {
                outputContainer.Remove(port);
                OutputPorts.Remove(port);
            }
        }

        private void HandleChoiceChanged(int index, string choiceText)
        {
            UpdatePortName(index, choiceText);
        }

        public void UpdateTitle(string title = null)
        {
            if (string.IsNullOrEmpty(title))
            {
                var name = Node.mainBehaviour != null
                                ? Node.mainBehaviour.GetType().Name
                                : string.Empty;

                if (name.Contains("Behaviour"))
                    name = name.Split("Behaviour")[0];

                this.title = name;
            }
            else
                this.title = title;
        }

        private void CreateInputPorts()
        {
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
            InputPort.portColor = Color.cyan;
            InputPort.portName = "in";

            inputContainer.Add(InputPort);
        }

        private void CreateOutputPorts()
        {
            outputContainer.Clear();
            OutputPorts.Clear();

            var ports = Node.nextNodes;

            if(ports == null || ports.Count == 0)
            {
                ports = new List<string>
                {
                    string.Empty
                };

                Node.nextNodes = ports;
            }

            if(Node.mainBehaviour is Choice choice)
            {
                foreach (var ch in choice.choices)
                {
                    var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);

                    outputPort.portName = ch;
                    outputPort.portColor = Color.cyan;

                    OutputPorts.Add(outputPort);

                    outputContainer.Add(outputPort);
                }

                return;
            }

            foreach (var port in ports)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);

                outputPort.portName = "out";
                outputPort.portColor = Color.cyan;

                OutputPorts.Add(outputPort);

                outputContainer.Add(outputPort);
            }
        }

        private void CreateVariableContainer()
        {
            if (Node.mainBehaviour == null)
                return;

            m_VariablesContainer.Clear();

            var type = Node.mainBehaviour.GetType();
            var displayFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(f => f.GetCustomAttribute<DisplayOnNodeAttribute>() != null);

            if (displayFields != null && displayFields.Count() > 0)
            {
                var serializedObject = new SerializedObject(Node.mainBehaviour);

                foreach (var f in displayFields)
                {
                    var serializedProp = serializedObject.FindProperty(f.Name);
                    var propertyField = new PropertyField(serializedProp, serializedProp.displayName);
                    propertyField.BindProperty(serializedObject);
                    propertyField.label = serializedProp.displayName;
                    m_VariablesContainer.Add(propertyField);
                }

                var style = m_VariablesContainer.style;
                style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
        }

        private void SetupStyleClasses()
        {
            if (Node.mainBehaviour == null)
                return;

            switch (Node.mainBehaviour.GetType().Name)
            {
                case "Replica":
                    this.titleContainer.AddToClassList("replica");
                    break;
                default:
                    break;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if(Node.mainBehaviour is Choice choice)
            {
                ChoiceEditor.OnChoiceAdded -= HandleChoiceAdded;
                ChoiceEditor.OnChoiceAdded += HandleChoiceAdded;

                ChoiceEditor.OnChoiceRemoved -= HandleChoiceRemoved;
                ChoiceEditor.OnChoiceRemoved += HandleChoiceRemoved;

                ChoiceEditor.OnChoiceChanged -= HandleChoiceChanged;
                ChoiceEditor.OnChoiceChanged += HandleChoiceChanged;
            }

            OnNodeSelected?.Invoke(this);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Undo.RecordObject(Node, $"Node '{Guid}' (Set Position)");

            Node.position = new Vector2(newPos.xMin, newPos.yMin);

            EditorUtility.SetDirty(Node);
        }
    }
}