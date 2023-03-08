using Self.Articy;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections;

namespace Self.Story.Editors
{
    public class NodeEditorView : GraphView
    {
        // required by GraphView
        public new class UxmlFactory : UxmlFactory<NodeEditorView, GraphView.UxmlTraits> { } 

        public event Action<NodeView> OnNodeSelected;

        public StoryEditorWindow EditorWindow { get; set; }

        private Chapter m_CurrentChapter;
        private List<Node> m_ChildNodes;

        private List<Node> m_NodesToCopy = new List<Node>();



        #region CONSTRUCTOS

        public NodeEditorView()
        {
            Insert(0, new GridBackground());

            var contentZoomer = new ContentZoomer();

            contentZoomer.minScale = 0.15f;
            contentZoomer.referenceScale = 2f;
            contentZoomer.maxScale = 3f;

            this.AddManipulator(contentZoomer);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Game/Modules/Core.Story/StoryEditor/Styles/EditorStyle.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
            RegisterCallback<KeyDownEvent>(HandleKeyboard);

            this.serializeGraphElements += CopyOperation;
            this.unserializeAndPaste += PasteOperation;
        }

        public void Create(Chapter chapter)
        {
            m_CurrentChapter = chapter;

            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements.ToList());

            graphViewChanged += OnGraphViewChanged;

            if (m_CurrentChapter.nodes == null)
                m_CurrentChapter.nodes = new Dictionary<string, Node>();

            m_ChildNodes = m_CurrentChapter.nodes.Values.ToList();

            m_ChildNodes.ForEach(CreateNodeView);

            m_ChildNodes.ForEach(n =>
            {
                var parentView = FindNodeView(n.id);

                if (n.nextNodes != null && n.nextNodes.Count > 0)
                {
                    for (int i = 0; i < n.nextNodes.Count; i++)
                    {
                        if (string.IsNullOrEmpty(n.nextNodes[i]))
                            continue;

                        var outputNode = FindNodeView(n.nextNodes[i]);
                        var outputPort = parentView.OutputPorts[i];
                        var inputPort = outputNode.InputPort;
                        var edge = outputPort.ConnectTo(inputPort);

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
            catch (InvalidOperationException ex)
            {
                Create(m_CurrentChapter);
            }

            return args;
        }

        private void OnEdgeCreated(Edge edge)
        {
            var parentView = edge.output.node as NodeView;
            var childView = edge.input.node as NodeView;

            var outputPortIndex = parentView.OutputPorts.IndexOf(edge.output);

            StoryEditorWindow.ConnectNode(parentView.Node, outputPortIndex, childView.Node.id);
        }

        private void OnGraphElementRemoved(GraphElement elem)
        {
            var nodeView = elem as NodeView;

            if (nodeView != null)
            {
                StoryEditorWindow.DeleteNode(nodeView.Node, m_CurrentChapter);
            }

            Edge edge = elem as Edge;

            if (edge != null && edge.output != null && edge.input != null)
            {
                var parentView = edge.output.node as NodeView;

                var outputPortIndex = parentView.OutputPorts.IndexOf(edge.output);
                parentView.Node.nextNodes[outputPortIndex] = string.Empty;
            }
        }

        private void CreateNodeView(Node node)
        {
            try
            {
                var view = new NodeView(node);
                view.OnNodeSelected += OnNodeSelected;

                AddElement(view);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateNode(Type a, Vector2 position)
        {
            var worldMousePosition = EditorWindow.rootVisualElement.ChangeCoordinatesTo(EditorWindow.rootVisualElement.parent, position - EditorWindow.position.position);
            var localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition) + new Vector2(150, 80);

            Node node = StoryEditorWindow.CreateNode(a, m_CurrentChapter, localMousePosition);

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
            //var allTypes = TypeCache.GetTypesDerivedFrom<Node>();
            var allTypes = new List<Type> { typeof(Node) };
            var concreteType = allTypes.Where(t => !t.IsAbstract);

            foreach (var t in concreteType)
            {
                //NodeInfoAttribute metadata = (NodeInfoAttribute)t.GetCustomAttributes(typeof(NodeInfoAttribute), true).FirstOrDefault(x => x is NodeInfoAttribute);

                //if (metadata != null)
                //    evt.menu.AppendAction($"{metadata.MenuName}", a => CreateNode(t, a.eventInfo.mousePosition));
                //else
                //    UnityEngine.Debug.LogError($"Node {t.Name} is missing NodeInfoAttribute");

                evt.menu.AppendAction($"Create/{t.Name}", a => CreateNode(t, a.eventInfo.mousePosition));
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
                    ((nda.CanAdapt(startPort.source, endPort.source) && nda.Connect(startPort.source, endPort.source)) ||
                    (endPort.portType == startPort.portType)))
                    .ToList();

            return validPorts;
        }

        #endregion

        #region NODES COPY/PASTE

        private void DuplicateSelection(List<ISelectable> selection)
        {
            List<Node> nodes = new List<Node>();

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

        private void PasteNodes(List<Node> nodesToDuplicate)
        {
            List<string> clonedGuids = new List<string>();

            Dictionary<string, string> clonedGuidToOldGuid = new Dictionary<string, string>();
            Dictionary<string, string> oldGuidToClonedGuid = new Dictionary<string, string>();

            List<Node> duplicatedNodes = new List<Node>();

            foreach (Node originalNode in nodesToDuplicate)
            {
                Node clone = StoryEditorWindow.CreateNode(originalNode.GetType(), m_CurrentChapter, originalNode.position);

                CopyNodeFields(originalNode, clone);

                clone.nextNodes = new List<string>();
                clone.position = clone.position + new Vector2(20, -20);

                clonedGuidToOldGuid.Add(clone.id, originalNode.id);
                oldGuidToClonedGuid.Add(originalNode.id, clone.id);

                clonedGuids.Add(clone.id);

                duplicatedNodes.Add(clone);
            }

            foreach (var node in duplicatedNodes)
            {
                var originalId = clonedGuidToOldGuid[node.id];
                var originalNode = m_CurrentChapter.nodes[originalId];

                if (originalNode.nextNodes != null && node.nextNodes != null)
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

                        if(oldGuidToClonedGuid.ContainsKey(originalNode.nextNodes[i]))
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

        private void CopyNodeFields(Node src, Node dst)
        {
            if (!src.GetType().Equals(dst.GetType()))
                return;

            var type = src.GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                if (f.Name.Equals("id"))
                    continue;

                var value = f.GetValue(src);

                if (value != null && value.GetType().GetInterface(nameof(ICloneable)) != null)
                {
                    var cloneMethod = value.GetType().GetMethod(nameof(ICloneable.Clone));
                    var clonedValue = cloneMethod.Invoke(value, null);

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