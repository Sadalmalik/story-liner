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



        public static NodeView Create(Node node)
        {
            var uiFileAsset = Resources.Load("Styles/StoryNodeView");
            var uiFilePath = AssetDatabase.GetAssetPath(uiFileAsset);

            var newNode = new NodeView(uiFilePath);

            newNode.Node = node;
            newNode.viewDataKey = node.id;
            newNode.Guid = node.id;

            var style = newNode.style;

            style.left = node.position.x;
            style.top = node.position.y;

            newNode.m_VariablesContainer = newNode.Q<VisualElement>("variables-container");

            var debugInfo = newNode.Q<Label>("debug-info");
            debugInfo.text = $"id:{node.id}";

            newNode.CreateVariableContainer();

            newNode.CreateInputPorts();
            newNode.CreateOutputPorts();
            newNode.SetupStyleClasses();
            newNode.UpdateTitle();

            return newNode;
        }

        public NodeView(string uiFile) : base(uiFile) { }

        public void UpdatePortName(int index, string name)
        {
            OutputPorts[index].portName = name;
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
            //if (Node.mainBehaviour == null)
            //    return;

            //m_VariablesContainer.Clear();

            //var type = Node.mainBehaviour.GetType();
            //var displayFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            //                        .Where(f => f.GetCustomAttribute<DisplayOnNodeAttribute>() != null);

            //if (displayFields != null && displayFields.Count() > 0)
            //{
            //    var serializedObject = new SerializedObject(Node.mainBehaviour);

            //    foreach (var f in displayFields)
            //    {
            //        var serializedProp = serializedObject.FindProperty(f.Name);
            //        var propertyField = new PropertyField(serializedProp, serializedProp.displayName);
            //        propertyField.BindProperty(serializedObject);
            //        propertyField.label = serializedProp.displayName;
            //        m_VariablesContainer.Add(propertyField);
            //    }

            //    var style = m_VariablesContainer.style;
            //    style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            //}
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