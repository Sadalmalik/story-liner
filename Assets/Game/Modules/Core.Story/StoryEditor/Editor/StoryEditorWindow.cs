using Self.Articy;
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
        private InspectorView m_InspectorView;
        public ToolbarToggle m_DebugInfoToggle { get; set; }

        [SerializeField] private Chapter m_CurrentChapter; // to prevent editor window flushing after recompling



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
            if (Selection.activeObject is Chapter chapter)
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

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Game/Modules/Core.Story/StoryEditor/Styles/EditorWindow.uxml");
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Game/Modules/Core.Story/StoryEditor/Styles/EditorStyle.uss");
            root.styleSheets.Add(styleSheet);

            m_EditorView = root.Q<NodeEditorView>();
            m_EditorView.EditorWindow = this;
            m_EditorView.OnNodeSelected += HandleNodeSelected;

            m_Toolbar = root.Q<Toolbar>();
            m_InspectorView = root.Q<InspectorView>();

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

        public static Node CreateNode(System.Type type, Chapter chapter, Vector2 position)
        {
            var newNode = ScriptableObject.CreateInstance(type) as Node;
            newNode.id = GUID.Generate().ToString();
            newNode.position = position;
            newNode.nextNodes = new List<string>();

            var croppedId = newNode.id.Substring(0, 6);
            newNode.name = GetNodeName(newNode, typeof(Replica));

            var defaultMainBehaviour = (Replica)(ScriptableObject.CreateInstance(typeof(Replica)));
            defaultMainBehaviour.name = GetNodeMainBehaviourName(newNode, typeof(Replica));

            Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Add Node)");

            chapter.nodes.Add(newNode.id, newNode);

            AssetDatabase.AddObjectToAsset(newNode, chapter);

            Undo.RegisterCreatedObjectUndo(newNode, $"Chapter '{chapter.chapterName}' (Add Node)");

            AssetDatabase.SaveAssets();

            return newNode;
        }

        public static void DeleteNode(Node target, Chapter chapter)
        {
            Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Delete Node)");

            if(target != null)
            {
                chapter.nodes.Remove(target.id);
                AssetDatabase.RemoveObjectFromAsset(target);

                if (target.mainBehaviour != null)
                    AssetDatabase.RemoveObjectFromAsset(target.mainBehaviour);

                if(target.behaviours != null && target.behaviours .Count > 0)
                {
                    foreach (var behAsset in target.behaviours)
                    {
                        AssetDatabase.RemoveObjectFromAsset(behAsset);
                    }
                }

                Undo.DestroyObjectImmediate(target);
            }

            var brokenNodes = new List<string>();

            foreach (var node in chapter.nodes)
            {
                if (node.Value == null)
                    brokenNodes.Add(node.Key);
            }

            foreach (var node in brokenNodes)
            {
                chapter.nodes.Remove(node);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ConnectNode(Node parent, int index, string nextNodeId)
        {
            parent.nextNodes[index] = nextNodeId;

            AssetDatabase.SaveAssets();
        }

        public static string GetNodeName(Node node, Type behaviourType)
        {
            var croppedId = node.id.Substring(0, 6);

            return $"node.{croppedId}.{behaviourType.Name.Replace("Behaviour", string.Empty)}";
        }

        public static string GetNodeMainBehaviourName(Node node, Type behaviourType)
        {
            var croppedId = node.id.Substring(0, 6);

            return $"node.{croppedId}.main.{behaviourType.Name.Replace("Behaviour", string.Empty)}";
        }

        public static string GetNodeSubBehaviourName(Node node, Type behaviourType, int index)
        {
            var croppedId = node.id.Substring(0, 6);

            return $"node.{croppedId}.sub.{index}.{behaviourType.Name.Replace("Behaviour", string.Empty)}";
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
            m_InspectorView.UpdateSelection(selectedNodeView);
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
            if (Selection.activeObject is Chapter
                || Selection.activeObject is Node
                || Selection.activeObject is NodeBehaviour)
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                var chapterAsset = (Chapter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Chapter));

                var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

                var nodes = assets
                                .Where(a => a is Node)
                                .Select(a => a as Node)
                                .ToList();

                var nodeBehaviours = assets
                                        .Where(a => a is NodeBehaviour)
                                        .Select(a => a as NodeBehaviour)
                                        .ToList();

                var assetsToRemove = new List<UnityEngine.Object>();

                foreach (var node in nodes)
                {
                    if (!chapterAsset.nodes.ContainsKey(node.id))
                    {
                        assetsToRemove.Add(node);
                    }
                }

                foreach (var nb in nodeBehaviours)
                {
                    var isMain = chapterAsset.nodes.Any(n => n.Value.mainBehaviour != null &&  n.Value.mainBehaviour == nb);
                    var isSub = chapterAsset.nodes.Any(n => n.Value.behaviours != null && n.Value.behaviours.Contains(nb));

                    if(!(isMain || isSub))
                    {
                        assetsToRemove.Add(nb);
                    }
                }

                foreach (var asset in assetsToRemove)
                {
                    AssetDatabase.RemoveObjectFromAsset(asset);
                }

                if (chapterAsset.nodes != null && chapterAsset.nodes.Any(n => n.Value == null))
                {
                    var nullNodes = chapterAsset.nodes
                                                    .Where(n => n.Value == null)
                                                    .ToList();

                    foreach (var nn in nullNodes)
                    {
                        chapterAsset.nodes.Remove(nn.Key);
                    }

                    EditorUtility.SetDirty(chapterAsset);
                }

                if (assetsToRemove.Count > 0)
                    AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(FixNullNodes)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            }
        }

        [MenuItem("StoryEditor/Fix Nodes Names")]
        public static void FixNodesNames()
        {
            if (Selection.activeObject is Chapter
                || Selection.activeObject is Node
                || Selection.activeObject is NodeBehaviour)
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                var chapterAsset = (Chapter)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Chapter));

                var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

                var nodes = assets
                                .Where(a => a is Node)
                                .Select(a => a as Node)
                                .ToList();

                var fixedNames = false;

                foreach (var node in nodes)
                {
                    if(node.mainBehaviour != null)
                    {
                        node.name = GetNodeName(node, node.mainBehaviour.GetType());
                        EditorUtility.SetDirty(node);
                        fixedNames = true;
                    }
                }

                if (fixedNames)
                {
                    EditorUtility.SetDirty(chapterAsset);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(FixNullNodes)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            }
        }

        [MenuItem("StoryEditor/Check Asset Path")]
        private static void CheckAssetPath()
        {
            if (Selection.activeObject is Chapter 
                || Selection.activeObject is Node
                || Selection.activeObject is NodeBehaviour)
            {
                Debug.Log($"[{nameof(StoryEditorWindow)}.{nameof(CheckAssetPath)}] {AssetDatabase.GetAssetPath(Selection.activeObject)}");

                var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
                var assetsList = new StringBuilder();

                foreach (var asset in assets)
                {
                    assetsList.AppendLine(asset.name);
                }

                Debug.Log(assetsList.ToString());

            }
            else
            {
                Debug.LogError($"[{nameof(StoryEditorWindow)}.{nameof(CheckAssetPath)}] Not a Chapter, Node or NodeBehaviour asset selected!");
            }
        }

        #endregion
    }
}