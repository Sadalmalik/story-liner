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
		public event Action<NodeView>      OnNodeSelected;
		public event Action<NodeView, int> OnNodePortDisconnected;

		public BaseNode Node { get; private set; }
		public string   Guid { get; private set; }
		public Chapter  CurrentChapter { get; private set; }

		public Port       InputPort;
		public List<Port> OutputPorts = new List<Port>();

		private VisualElement         m_NodeInspector;
		private NodeMetadataAttribute m_NodeMetaData;



		public static NodeView Create(BaseNode node, Chapter chapter)
		{
			// To avoid absolute paths
			var uiFileAsset = Resources.Load("Styles/StoryNodeView");
			var uiFilePath  = AssetDatabase.GetAssetPath(uiFileAsset);

			// we use the static method
			// and instead feeding a found uiFilePath
			// to the default constructor
			var newNode = new NodeView(uiFilePath);

			newNode.CurrentChapter = chapter;

			newNode.Node           = node;
			newNode.viewDataKey    = node.id;
			newNode.Guid           = node.id;
			newNode.m_NodeMetaData = node.GetType().GetCustomAttribute(typeof(NodeMetadataAttribute)) as NodeMetadataAttribute;

			var style = newNode.style;

			style.left = node.position.x;
			style.top  = node.position.y;

			newNode.m_NodeInspector = newNode.Q<VisualElement>("inspector-container");

			var debugInfo = newNode.Q<Label>("debug-info");
			debugInfo.text = $"id:{node.id}";

			newNode.CreateInputPorts();
			newNode.CreateOutputPorts();
			newNode.CreateVariableContainer();
			newNode.SetupStyleClasses();
			newNode.UpdateTitle();

			return newNode;
		}

		// To avoid absolute paths
		public NodeView(string uiFile) : base(uiFile)
		{
		}

		public void UpdatePortName(int index, string name)
		{
			OutputPorts[index].portName = name;
		}

		public void UpdateTitle(string title = null)
		{
			if (string.IsNullOrEmpty(title))
			{
				var name = Node.GetType().Name.Replace("Node", string.Empty);

				this.title = name;
			}
			else
				this.title = title;
		}

		private void CreateInputPorts()
		{
			inputContainer.Clear();

			// only add default input if it's not a custom input
			if (m_NodeMetaData.customInput)
                return;

            InputPort           = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
			InputPort.portColor = Color.cyan;
			InputPort.portName  = "in";
			InputPort.AddToClassList("left-port");

			inputContainer.Add(InputPort);
		}

		private void CreateOutputPorts()
		{
			outputContainer.Clear();
			OutputPorts.Clear();

			// only add default outputs if it's not a custom output
			if (m_NodeMetaData.customOutput)
				return;

			var ports = Node.nextNodes;

			if (ports == null || ports.Count == 0)
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

				outputPort.portName  = "out";
				outputPort.portColor = Color.cyan;
				outputPort.AddToClassList("right-port");

				OutputPorts.Add(outputPort);

				outputContainer.Add(outputPort);
			}
		}

		private void CreateVariableContainer()
		{
			var editors = TypeCache
				.GetTypesDerivedFrom(typeof(NodeEditor))
				.ToList();

			try
			{
				var nodeEditorType = editors.First(t =>
				{
					var typeAttrib = t.GetCustomAttribute<InspectedTypeAttribute>();

					if (typeAttrib != null && typeAttrib.inspectedType == Node.GetType())
						return true;

					return false;
				});

				var nodeEditor = (NodeEditor) Editor.CreateEditor(Node, nodeEditorType);

				nodeEditor.CreateInspectorGUI(m_NodeInspector, this);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);

				var nodeEditor = (NodeEditor) Editor.CreateEditor(Node, typeof(NodeEditor));

				m_NodeInspector.Add(nodeEditor.CreateInspectorGUI());
			}
		}

		private void SetupStyleClasses()
		{
            switch (Node.GetType().Name)
            {
				case "ConditionNode":
					titleContainer.AddToClassList("condition-node");
					break;
				case "ActiveNode":
					titleContainer.AddToClassList("active-node");
					break;
				case "ChoiceNode":
					titleContainer.AddToClassList("choice-node");
					break;
				case "ReplicaNode":
					titleContainer.AddToClassList("replica-node");
					break;
				case "EntryNode":
					titleContainer.AddToClassList("entry-node");
					break;
				case "ExitNode":
					titleContainer.AddToClassList("exit-node");
					break;
				case "ChapterNode":
					titleContainer.AddToClassList("chapter-node");
					break;
				case "DragItemNode":
					titleContainer.AddToClassList("dragitem-node");
					break;
				default:
                    break;
            }
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Node;

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