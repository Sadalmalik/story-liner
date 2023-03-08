using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Self.Story.Editors
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public event Action<NodeView> OnNodeSelected;

        public Node Node { get; private set; }
        public string Guid { get; private set; }

        public Port InputPort;
        public List<Port> OutputPorts;



        public NodeView(Node node) : base("Assets/Game/Modules/Core.Story/StoryEditor/Styles/StoryNodeView.uxml")
        {
            this.Node = node;
            this.viewDataKey = node.id;
            this.Guid = node.id;

            style.left = node.position.x;
            style.top = node.position.y;

            //var content = this.contentContainer.Q<Label>("contentText");
            //content.text = node.Properties.Text;

            //CreateDisplayProperties();

            CreateInputPorts();
            CreateOutputPorts();
            SetupStyleClasses();
            UpdateTitle();
        }

        public void UpdateTitle(string title = null)
        {
            if (string.IsNullOrEmpty(title))
                this.title = Node.mainBehaviour != null ? Node.mainBehaviour.GetType().Name : string.Empty;
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

            var ports = Node.nextNodes;
            OutputPorts = new List<Port>();

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