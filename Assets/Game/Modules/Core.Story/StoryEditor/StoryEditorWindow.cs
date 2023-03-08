using Self.Articy;
using System.Collections.Generic;
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
            newNode.name = type.Name;
            newNode.id = GUID.Generate().ToString();
            newNode.position = position;
            newNode.nextNodes = new List<string>();

            Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Add Node)");

            chapter.nodes.Add(newNode.id, newNode);

            if (!Application.isPlaying)
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

        #endregion
    }
}