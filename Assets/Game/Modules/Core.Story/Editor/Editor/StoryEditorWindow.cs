using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    public class StoryEditorWindow : EditorWindow
    {
        private Toolbar m_Toolbar;
        private ToolbarMenu m_ToolbarMenu;
        private static StoryEditorWindow m_CachedWindow;
        private NodeEditorView m_EditorView;
        //private InspectorView m_InspectorView;
        public ToolbarToggle m_DebugInfoToggle { get; set; }

        [SerializeField] private StoryV2.Chapter m_CurrentChapter; // to prevent editor window flushing after recompling



        #region CONSTRUCTORS

        [MenuItem("StoryEditor/Editor Window")]
        public static StoryEditorWindow OpenWindow()
        {
            m_CachedWindow = GetWindow<StoryEditorWindow>();

            if (m_CachedWindow == null)
                m_CachedWindow = CreateWindow<StoryEditorWindow>();

            m_CachedWindow.titleContent = new GUIContent("Story Editor");
            m_CachedWindow.position = new Rect(160, 120, 1100, 630);

            return m_CachedWindow;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is StoryV2.Chapter chapter)
            {
                StoryEditorWindow wnd = OpenWindow();

                wnd.m_CurrentChapter = chapter; 
                wnd.m_EditorView.Create(chapter);

                return true;
            }

            return false;
        }

        public void CreateGUI()
        { 
            VisualElement root = rootVisualElement;

            var visualTree = Resources.Load<VisualTreeAsset>("Styles/EditorWindow");
            visualTree.CloneTree(root);

            m_EditorView = root.Q<NodeEditorView>();
            m_EditorView.EditorWindow = this;
            m_EditorView.OnNodeSelected += HandleNodeSelected;

            m_Toolbar = root.Q<Toolbar>();
            //m_InspectorView = root.Q<InspectorView>();

            m_DebugInfoToggle = root.Q<ToolbarToggle>("debug-info-toggle");
            m_DebugInfoToggle.RegisterValueChangedCallback(HandleDebugInfoToggle);

            var openAssetButton = m_Toolbar.Q<ToolbarButton>("ImportFromArticy");
            openAssetButton.clicked += HandleImportFromArticyButtonClick;

            m_ToolbarMenu = root.Q<ToolbarMenu>();

            if (m_CurrentChapter != null)
                m_EditorView.Create(m_CurrentChapter);

            EditorApplication.delayCall += () =>
            {
                m_EditorView.FrameAll();
            };
        }

        #endregion

        #region NODE ACTIONS

        public static StoryV2.Node CreateNode(Type type, StoryV2.Chapter chapter, Vector2 position)
        {
            var newNode = ScriptableObject.CreateInstance(type) as StoryV2.Node;
            newNode.id = GUID.Generate().ToString();
            newNode.position = position;
            newNode.nextNodes = new List<string>();

            newNode.name = GetNodeName(newNode);

            Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Add Node)");

            chapter.nodes.Add(newNode);
            EditorUtility.SetDirty(chapter);

            AssetDatabase.AddObjectToAsset(newNode, chapter);
            AssetDatabase.SaveAssets();

            return newNode;
        }

        public static void DeleteNode(StoryV2.Node target, StoryV2.Chapter chapter)
        {
            Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Delete Node)");

            if(target != null)
            {
                if(chapter.nodes.Any(n => n.id.Equals(target.id)))
                {
                    var nodeToRemove = chapter.nodes.First(n => n.id.Equals(target.id));

                    chapter.nodes.Remove(nodeToRemove);
                    AssetDatabase.RemoveObjectFromAsset(nodeToRemove);

                    if(nodeToRemove.behaviours != null && nodeToRemove.behaviours.Count > 0)
                    {
                        var behavioursToRemove = nodeToRemove.behaviours
                                                                .ToList();

                        foreach (var nb in behavioursToRemove)
                        {
                            AssetDatabase.RemoveObjectFromAsset(nb);
                        }
                    }

                    Undo.DestroyObjectImmediate(target);
                }
            }

            for (int i = chapter.nodes.Count - 1; i >= 0; i--)
            {
                if (chapter.nodes[i] == null)
                {
                    chapter.nodes.RemoveAt(i);
                    EditorUtility.SetDirty(chapter);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ConnectNode(StoryV2.Node parent, int index, string nextNodeId)
        {
            parent.nextNodes[index] = nextNodeId;

            AssetDatabase.SaveAssets();
        }

        public static string GetNodeName(StoryV2.Node node)
        {
            var croppedId = node.id.Substring(0, 6);
            var nodeType = node.GetType().Name.Replace("Node", string.Empty);

            return $"node.{croppedId}.{nodeType}";
        }

        public static string GetNodeActionName(StoryV2.Node node, Type actionType, int index)
        {
            var croppedId = node.id.Substring(0, 6);
            var nodeType = node.GetType().Name.Replace("Node", string.Empty);
            var nodeAction = actionType.Name.Replace("Action", string.Empty);

            return $"node.{croppedId}.{nodeType}.{nodeAction}.{index}";
        }

        #endregion

        #region HANDLE EVENTS

        private void HandleImportFromArticyButtonClick()
        {
            throw new System.NotImplementedException($"[{nameof(StoryEditorWindow)}.{nameof(HandleImportFromArticyButtonClick)}] Not implemented yet");

            //var file = EditorUtility.OpenFilePanel("Open File", Application.dataPath, "json");

            //if(string.IsNullOrEmpty(file))
            //    throw new System.Exception("Could not open file, path is null or empty");

            //var relativeFilePath = $"Assets/{file.Split("Assets/")[1]}";
            //var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeFilePath);

            //if (jsonAsset == null)
            //    throw new System.Exception($"Error loading asset at path {relativeFilePath}");

            //var data = ArticyProjectImporter.ImportFromJsonAsset(jsonAsset);
            //var storyNodes = new Dictionary<ulong, StoryNode>();

            //data.Packages[0].Models.ForEach(m => storyNodes.Add(m.Properties.Id.Value, m));

            //FillToolbarMenu(data);
        }

        private void HandleNodeSelected(NodeView selectedNodeView)
        {
            //m_InspectorView.UpdateSelection(selectedNodeView);
        }

        private void HandleDebugInfoToggle(ChangeEvent<bool> newState)
        {
            var nodes = m_EditorView
                            .graphElements
                            .ToList()
                            .Where(elem => elem is NodeView);

            var displayFlag = (DisplayStyle)(newState.newValue ? 0 : 1);

            foreach (var node in nodes)
            {
                var debugInfoText = node.Q<Label>("debug-info");
                var style = debugInfoText.style;
                style.display = new StyleEnum<DisplayStyle>(displayFlag);
            }
        }

        #endregion

        #region UTILITY

        [MenuItem("StoryEditor/Fix Null Nodes")]
        public static void FixNullNodes()
        {

            // TODO [Andrei]: Fix this after reworking the editor
            // to be compatible with the new structure

            //if (Selection.activeObject is Chapter
            //    || Selection.activeObject is StoryV2.Node
            //    || Selection.activeObject is NodeAction)
            //{
            //    var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            //    var chapterAsset = (Chapter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Chapter));

            //    var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

            //    var nodes = assets
            //                    .Where(a => a is Node)
            //                    .Select(a => a as Node)
            //                    .ToList();

            //    var nodeBehaviours = assets
            //                            .Where(a => a is NodeAction)
            //                            .Select(a => a as NodeAction)
            //                            .ToList();

            //    var assetsToRemove = new List<UnityEngine.Object>();

            //    foreach (var node in nodes)
            //    {
            //        if (!chapterAsset.nodeById.Any(n => n.id.Equals(node.id)))
            //        {
            //            assetsToRemove.Add(node);
            //        }
            //    }

            //    foreach (var nb in nodeBehaviours)
            //    {
            //        var isSub = chapterAsset.nodeById.Any(n => (n as StoryV2.Node).behaviours.Contains(nb));

            //        if(!isSub)
            //        {
            //            assetsToRemove.Add(nb);
            //        }
            //    }

            //    foreach (var asset in assetsToRemove)
            //    {
            //        AssetDatabase.RemoveObjectFromAsset(asset);
            //    }

            //    if (chapterAsset.nodeById != null && chapterAsset.nodeById.Any(n => n.Value == null))
            //    {
            //        var nullNodes = chapterAsset.nodeById
            //                                        .Where(n => n.Value == null)
            //                                        .ToList();

            //        foreach (var nn in nullNodes)
            //        {
            //            chapterAsset.nodeById.Remove(nn.Key);
            //        }

            //        EditorUtility.SetDirty(chapterAsset);
            //    }

            //    if (assetsToRemove.Count > 0)
            //        AssetDatabase.SaveAssets();
            //}
            //else
            //{
            //    Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(FixNullNodes)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            //}
        }

        [MenuItem("StoryEditor/Fix Nodes Names")]
        public static void FixNodesNames()
        {
            // TODO [Andrei]: Fix this after reworking the editor
            // to be compatible with the new structure

            //if (Selection.activeObject is Chapter
            //    || Selection.activeObject is Node
            //    || Selection.activeObject is NodeAction)
            //{
            //    var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            //    var chapterAsset = (Chapter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Chapter));

            //    var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

            //    var nodes = assets
            //                    .Where(a => a is Node)
            //                    .Select(a => a as Node)
            //                    .ToList();

            //    var fixedNames = false;

            //    foreach (var node in nodes)
            //    {
            //        if(node.mainBehaviour != null)
            //        {
            //            node.name = GetNodeName(node, node.mainBehaviour.GetType());
            //            EditorUtility.SetDirty(node);
            //            fixedNames = true;
            //        }
            //    }

            //    if (fixedNames)
            //    {
            //        EditorUtility.SetDirty(chapterAsset);
            //        AssetDatabase.SaveAssets();
            //    }
            //}
            //else
            //{
            //    Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(FixNullNodes)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            //}
        }

        [MenuItem("StoryEditor/Check Asset Path")]
        private static void CheckAssetPath()
        {
            // TODO [Andrei]: Fix this after reworking the editor
            // to be compatible with the new structure

            //if (Selection.activeObject is Chapter 
            //    || Selection.activeObject is Node
            //    || Selection.activeObject is NodeAction)
            //{
            //    Debug.Log($"[{nameof(StoryEditorWindow)}.{nameof(CheckAssetPath)}] {AssetDatabase.GetAssetPath(Selection.activeObject)}");

            //    var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
            //    var assetsList = new StringBuilder();

            //    foreach (var asset in assets)
            //    {
            //        assetsList.AppendLine(asset.name);
            //    }

            //    Debug.Log(assetsList.ToString());

            //}
            //else
            //{
            //    Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(CheckAssetPath)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            //}
        }

        #endregion
    }
}