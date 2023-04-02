using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

namespace Self.Story.Editors
{
	public class NodeEditorView : GraphView
	{
		// required by GraphView
		public new class UxmlFactory : UxmlFactory<NodeEditorView, GraphView.UxmlTraits>
		{
		}

		public event Action<NodeView> OnNodeSelected;

		public StoryEditorWindow EditorWindow { get; set; }

		private Chapter m_CurrentChapter;

		private List<BaseNode> m_NodesToCopy = new List<BaseNode>();


#region CONSTRUCTORS

		public NodeEditorView()
		{
			Insert(0, new GridBackground());

			var contentZoomer = new ContentZoomer();

			contentZoomer.minScale       = 0.15f;
			contentZoomer.referenceScale = 2f;
			contentZoomer.maxScale       = 3f;

			this.AddManipulator(contentZoomer);
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			var styleSheet = Resources.Load<StyleSheet>("Styles/EditorStyle");
			styleSheets.Add(styleSheet);

			Undo.undoRedoPerformed += OnUndoRedo;
			RegisterCallback<KeyDownEvent>(HandleKeyboard);

			this.serializeGraphElements += CopyOperation;
			this.unserializeAndPaste    += PasteOperation;
		}

		public void Create(Chapter chapter)
		{
			m_CurrentChapter = chapter;

			if (m_CurrentChapter != null)
				this.Q<Label>("current-chapter-name").text = m_CurrentChapter.chapterName;

			graphViewChanged -= OnGraphViewChanged;

			DeleteElements(graphElements.ToList());

			graphViewChanged += OnGraphViewChanged;

			var chapterNodes = m_CurrentChapter.nodes;

			chapterNodes.ForEach(CreateNodeView);

			chapterNodes.ForEach(n =>
			{
				var parentView = FindNodeView(n.id);

				if (n.nextNodes != null && n.nextNodes.Count > 0)
				{
					for (int i = 0; i < n.nextNodes.Count; i++)
					{
						if (string.IsNullOrEmpty(n.nextNodes[i]))
							continue;

						var outputNode = FindNodeView(n.nextNodes[i]);

						if (outputNode == null)
						{
							var brokenOutput = n.nextNodes[i].Clone();

							n.nextNodes[i] = string.Empty;

							Debug.LogWarning(
								$"[{nameof(NodeEditorView)}.{nameof(Create)}] Found broken output from node '{n.id}' to {brokenOutput}");
						}

						var outputPort = parentView.OutputPorts[i];
						var inputPort  = outputNode.InputPort;
						var edge       = outputPort.ConnectTo(inputPort);

						AddElement(edge);
					}
				}
			});
		}

#endregion

#region ELEMENTS CREATION

		private GraphViewChange OnGraphViewChanged(GraphViewChange args)
		{
			try
			{
				args.elementsToRemove?.ForEach(OnGraphElementRemoved);
				args.edgesToCreate?.ForEach(OnEdgeCreated);
			}
			catch (InvalidOperationException)
			{
				Create(m_CurrentChapter);
			}

			return args;
		}

		private void OnEdgeCreated(Edge edge)
		{
			var parentView = edge.output.node as NodeView;
			var childView  = edge.input.node as NodeView;

			var outputPortIndex = parentView.OutputPorts.IndexOf(edge.output);

			StoryEditorWindow.ConnectNode(parentView.Node, outputPortIndex, childView.Node.id);
		}

		private void OnGraphElementRemoved(GraphElement elem)
		{
			if (elem is Port port)
			{
				var edges = port.connections.ToList();

				DeleteElements(edges);
			}

			if (elem is NodeView nodeView)
			{
				StoryEditorWindow.DeleteNode(nodeView.Node, m_CurrentChapter);
			}

			if (elem is Edge edge)
			{
				if (edge.output != null && edge.input != null)
				{
					var parentView      = edge.output.node as NodeView;
					var outputPortIndex = parentView.OutputPorts.IndexOf(edge.output);

					StoryEditorWindow.DisconnectNode(parentView.Node, outputPortIndex);
				}
			}
		}

		private void CreateNodeView(BaseNode node)
		{
			try
			{
				var view = NodeView.Create(node, m_CurrentChapter);
				view.OnNodeSelected         += OnNodeSelected;
				view.OnNodePortDisconnected += HandleNodePortsDisconnected;

				var displayFlag   = (DisplayStyle) (EditorWindow.DebugInfoToggle.value ? 0 : 1);
				var debugInfoText = view.Q<Label>("debug-info");
				var style         = debugInfoText.style;
				style.display = new StyleEnum<DisplayStyle>(displayFlag);

				AddElement(view);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void HandleNodePortsDisconnected(NodeView view, int index)
		{
			var port  = view.OutputPorts[index];
			var edges = port.connections.ToArray();

			DeleteElements(edges);
		}

		private void CreateNode(Type a, Vector2 position)
		{
			var worldMousePosition =
				EditorWindow.rootVisualElement.ChangeCoordinatesTo(EditorWindow.rootVisualElement.parent,
					position - EditorWindow.position.position);
			var localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition) + new Vector2(150, 80);

			BaseNode node = StoryEditorWindow.CreateNode(a, m_CurrentChapter, localMousePosition);

			CreateNodeView(node);
		}

		private NodeView FindNodeView(string id)
		{
			return GetNodeByGuid(id) as NodeView;
		}

#endregion

#region EDITOR STUFF

		private void OnUndoRedo()
		{
			Create(m_CurrentChapter);

			AssetDatabase.SaveAssets();
		}

		private void HandleKeyboard(KeyDownEvent callback)
		{
			switch (callback.keyCode)
			{
				case KeyCode.D:
					if (!callback.ctrlKey)
						return;

					DuplicateSelection(selection);
					break;
				case KeyCode.Space:
					//SearchWindow.Open(new SearchWindowContext(Event.current.mousePosition), provider);
					break;
			}
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			var allTypes     = TypeCache.GetTypesDerivedFrom<BaseNode>();
			var concreteType = allTypes.Where(t => !t.IsAbstract);

			foreach (var t in concreteType)
			{
				//NodeInfoAttribute metadata = (NodeInfoAttribute)t.GetCustomAttributes(typeof(NodeInfoAttribute), true).FirstOrDefault(x => x is NodeInfoAttribute);

				//if (metadata != null)
				//    evt.menu.AppendAction($"{metadata.MenuName}", a => CreateNode(t, a.eventInfo.mousePosition));
				//else
				//    UnityEngine.Debug.LogError($"Node {t.Name} is missing NodeInfoAttribute");

				evt.menu.AppendAction(
					$"Create/{t.Name}",
					action => CreateNode(t, action.eventInfo.mousePosition));
			}
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			CustomNodeAdapter nda = new CustomNodeAdapter();

			var validPorts = ports
				.ToList()
				.Where(endPort =>
					endPort.direction != startPort.direction &&
					endPort.node != startPort.node &&
					((nda.CanAdapt(startPort.source, endPort.source) &&
					  nda.Connect(startPort.source, endPort.source)) ||
					 (endPort.portType == startPort.portType)))
				.ToList();

			return validPorts;
		}

#endregion

#region NODES COPY/PASTE

		private void DuplicateSelection(List<ISelectable> selection)
		{
			List<BaseNode> nodes = new List<BaseNode>();

			selection.ForEach(x =>
			{
				if (x is NodeView)
					nodes.Add((x as NodeView).Node);
			});

			PasteNodes(nodes);
		}

		private string CopyOperation(IEnumerable<GraphElement> elements)
		{
			m_NodesToCopy.Clear();

			foreach (GraphElement n in elements)
			{
				NodeView nodeView = n as NodeView;

				if (nodeView != null)
					m_NodesToCopy.Add(nodeView.Node);
			}

			return "Copy Nodes";
		}

		private void PasteOperation(string operationName, string data)
		{
			if (operationName != "Paste")
				return;

			PasteNodes(m_NodesToCopy);
		}

		private void PasteNodes(List<BaseNode> nodesToDuplicate)
		{
			List<string> clonedGuids = new List<string>();

			Dictionary<string, string> clonedGuidToOldGuid = new Dictionary<string, string>();
			Dictionary<string, string> oldGuidToClonedGuid = new Dictionary<string, string>();

			List<BaseNode> duplicatedNodes = new List<BaseNode>();

			foreach (var originalNode in nodesToDuplicate)
			{
				var clone = StoryEditorWindow.CreateNode(originalNode.GetType(), m_CurrentChapter,
					originalNode.position);

				CopyNodeFields(originalNode, clone);

				clone.nextNodes = new List<string>();
				clone.position  = clone.position + new Vector2(20, -20);

				clonedGuidToOldGuid.Add(clone.id, originalNode.id);
				oldGuidToClonedGuid.Add(originalNode.id, clone.id);

				clonedGuids.Add(clone.id);

				duplicatedNodes.Add(clone);
			}

			foreach (var node in duplicatedNodes)
			{
				var originalId   = clonedGuidToOldGuid[node.id];
				var originalNode = m_CurrentChapter.nodes.FirstOrDefault(n => n.id.Equals(originalId));

				if (originalNode != null && originalNode.nextNodes != null && node.nextNodes != null)
				{
					int portCountDiff = originalNode.nextNodes.Count - node.nextNodes.Count;

					for (int i = 0; i < portCountDiff; i++)
					{
						node.nextNodes.Add(null);
					}

					for (int i = 0; i < originalNode.nextNodes.Count; i++)
					{
						if (string.IsNullOrEmpty(originalNode.nextNodes[i]))
							continue;

						if (oldGuidToClonedGuid.ContainsKey(originalNode.nextNodes[i]))
							node.nextNodes[i] = oldGuidToClonedGuid[originalNode.nextNodes[i]];
					}
				}
			}

			Create(m_CurrentChapter);

			foreach (var guid in clonedGuids)
			{
				NodeView nv = GetNodeByGuid(guid) as NodeView;

				AddToSelection(nv);
			}

			m_NodesToCopy.Clear();
		}

		private void CopyNodeFields(BaseNode src, BaseNode dst)
		{
			if (!src.GetType().Equals(dst.GetType()))
				return;

			var type = src.GetType();
			var fields = type.GetFields(System.Reflection.BindingFlags.Instance |
			                            System.Reflection.BindingFlags.Public |
			                            System.Reflection.BindingFlags.NonPublic);

			var croppedIdSrc = src.id.Substring(0, 6);
			var croppedIdDst = dst.id.Substring(0, 6);

			foreach (var f in fields)
			{
				if (f.Name.Equals("id"))
					continue;

				var value = f.GetValue(src);

				if (value != null && value.GetType().GetInterface(nameof(ICloneable)) != null)
				{
					var cloneMethod = value.GetType().GetMethod(nameof(ICloneable.Clone));
					var clonedValue = cloneMethod.Invoke(value, null);

					if (clonedValue is BaseAction beh)
					{
						beh.name = beh.name.Replace(croppedIdSrc, croppedIdDst);

						AssetDatabase.AddObjectToAsset(beh, dst);
						AssetDatabase.SaveAssets();
					}

					f.SetValue(dst, clonedValue);
				}
				else
				{
					f.SetValue(dst, value);
				}
			}
		}

#endregion
	}
}