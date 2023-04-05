
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
	public class StoryEditorWindow : EditorWindow
	{
		public static StoryEditorWindow Instance => m_CachedWindow;

		private        Toolbar           m_Toolbar;
		private        ToolbarMenu       m_ToolbarMenu;
		private static StoryEditorWindow m_CachedWindow;

		public NodeEditorView EditorView { get; set; }

		//private InspectorView m_InspectorView;
		public ToolbarToggle DebugInfoToggle { get; set; }

		[SerializeField] private Chapter m_CurrentChapter; // to prevent editor window flushing after recompling


#region CONSTRUCTORS

		public static StoryEditorWindow OpenEmptyWindow()
		{
			m_CachedWindow = GetWindow<StoryEditorWindow>();

			if (m_CachedWindow == null)
				m_CachedWindow = CreateWindow<StoryEditorWindow>();

			m_CachedWindow.titleContent = new GUIContent("Story Editor");
			m_CachedWindow.position     = new Rect(160, 120, 1100, 630);

			return m_CachedWindow;
		}

		[MenuItem("StoryEditor/Editor Window")]
		public static StoryEditorWindow OpenWindowWithFile()
		{
			var assetPath = EditorUtility.OpenFilePanel("Select Chapter File", Application.dataPath, "asset");

			if (string.IsNullOrEmpty(assetPath))
				return null;

			var asset = LoadChapterAsset(assetPath);

			if (asset == null)
				return null;

			var window = OpenEmptyWindow();

			window.m_CurrentChapter = asset;
			window.EditorView.Create(asset);

			return window;
		}

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			if (Selection.activeObject is Chapter chapter)
			{
				StoryEditorWindow wnd = OpenEmptyWindow();

				CheckChapterVariablesContainer(chapter);

				wnd.m_CurrentChapter = chapter;
				wnd.EditorView.Create(chapter);

				return true;
			}

			return false;
		}

		public void CreateGUI()
		{
			m_CachedWindow = this;

			VisualElement root = rootVisualElement;

			var visualTree = Resources.Load<VisualTreeAsset>("Styles/EditorWindow");
			visualTree.CloneTree(root);

			EditorView                =  root.Q<NodeEditorView>();
			EditorView.EditorWindow   =  this;
			EditorView.OnNodeSelected += HandleNodeSelected;

			m_Toolbar = root.Q<Toolbar>();
			//m_InspectorView = root.Q<InspectorView>();

			DebugInfoToggle = root.Q<ToolbarToggle>("debug-info-toggle");
			DebugInfoToggle.RegisterValueChangedCallback(HandleDebugInfoToggle);

			var openAssetButton = m_Toolbar.Q<ToolbarButton>("ImportFromArticy");
			openAssetButton.clicked += HandleImportFromArticyButtonClick;

			m_ToolbarMenu = root.Q<ToolbarMenu>();

			if (m_CurrentChapter != null)
			{
				if (m_CurrentChapter.book.variables == null)
				{
					CheckChapterVariablesContainer(m_CurrentChapter);
				}

				EditorView.Create(m_CurrentChapter);
			}

			EditorApplication.delayCall += () => { EditorView.FrameAll(); };
		}


#endregion

#region NODE ACTIONS

		public static BaseNode CreateNode(Type type, Chapter chapter, Vector2 position)
		{
			var newNode = ScriptableObject.CreateInstance(type) as BaseNode;
			newNode.id        = GUID.Generate().ToString();
			newNode.position  = position;
			newNode.UpdateName();

			Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Add Node)");

			chapter.nodes.Add(newNode);
			EditorUtility.SetDirty(chapter);

			AssetDatabase.AddObjectToAsset(newNode, chapter);
			AssetDatabase.SaveAssets();

			return newNode;
		}

		public static void DeleteNode(BaseNode target, Chapter chapter)
		{
			Undo.RecordObject(chapter, $"Chapter '{chapter.chapterName}' (Delete Node)");

			if (target != null)
			{
				if (chapter.nodes.Any(n => n.id.Equals(target.id)))
				{
					var nodeToRemove = chapter.nodes.First(n => n.id.Equals(target.id));

					chapter.nodes.Remove(nodeToRemove);
					AssetDatabase.RemoveObjectFromAsset(nodeToRemove);

					var childObjects = GetChildObjects(nodeToRemove);

					if (childObjects == null)
						return;

                    foreach (var child in childObjects)
					{
						AssetDatabase.RemoveObjectFromAsset(child);
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

        private static List<UnityEngine.Object> GetChildObjects(BaseNode nodeToRemove)
        {
			var nodeType = nodeToRemove.GetType();

			var fieldSelector = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

			var fieldsThatAreSerializedAsChilds = nodeType.GetFields(fieldSelector)
														  .Where(f => f.GetCustomAttributes(typeof(SerializedChild), true) != null);

			if(fieldsThatAreSerializedAsChilds != null)
            {
				var objects = new List<UnityEngine.Object>();

                foreach (var f in fieldsThatAreSerializedAsChilds)
                {
					var obj = f.GetValue(nodeToRemove);

					if (obj != null)
						objects.Add(obj as UnityEngine.Object);
                }

				return objects;
            }

			return null;
        }

        public static void ConnectNode(BaseNode node, int index, string nextNodeId)
		{
			node.nextNodes[index] = nextNodeId;

			AssetDatabase.SaveAssets();
		}

		public static void DisconnectNode(BaseNode node, int index)
		{
			if (index < 0 || index >= node.nextNodes.Count)
				return;

			node.nextNodes[index] = string.Empty;

			AssetDatabase.SaveAssets();
		}

		public static string GetNodeActionName(BaseNode node, Type actionType, int index)
		{
			var croppedId  = node.id.Substring(0, 6);
			var nodeType   = node.GetType().Name.Replace("Node", string.Empty);
			var nodeAction = actionType.Name.Replace("Action", string.Empty);

			return $"node.{croppedId}.{nodeType}.{nodeAction}.{index}";
		}

#endregion

#region HANDLE EVENTS

		private void HandleImportFromArticyButtonClick()
		{
			throw new System.NotImplementedException(
				$"[{nameof(StoryEditorWindow)}.{nameof(HandleImportFromArticyButtonClick)}] Not implemented yet");

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
			var nodes = EditorView
				.graphElements
				.ToList()
				.Where(elem => elem is NodeView);

			var displayFlag = (DisplayStyle) (newState.newValue ? 0 : 1);

			foreach (var node in nodes)
			{
				var debugInfoText = node.Q<Label>("debug-info");
				var style         = debugInfoText.style;
				style.display = new StyleEnum<DisplayStyle>(displayFlag);
			}
		}

#endregion

#region UTILITY

		private static void CheckChapterVariablesContainer(Chapter target)
		{
			if (target.book.variables != null)
				return;

			var variablesContainer = (VariablesContainer) ScriptableObject.CreateInstance(typeof(VariablesContainer));
			variablesContainer.name      = ".settings.variables";

			var newAssetPath = AssetDatabase.GetAssetPath(target);

			AssetDatabase.AddObjectToAsset(variablesContainer, newAssetPath);

			target.book.variables = variablesContainer;

			AssetDatabase.SaveAssets();
		}

		private static Chapter LoadChapterAsset(string path)
        {
            try
            {
				var pathRegex = new Regex("(Assets/[\\w\\W]+)");
				var pathMatch = pathRegex.Match(path);

				var relativePath = pathMatch.Captures[0].Value;

				return AssetDatabase.LoadAssetAtPath<Chapter>(relativePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong :()");
            }
        }

#endregion
	}
}