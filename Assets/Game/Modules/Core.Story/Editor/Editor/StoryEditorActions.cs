using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Self.Story.Editors
{
	public static class StoryEditorActions
	{
		[MenuItem("Assets/[SELF]/Create Book")]
		public static void CreateBook()
		{
			var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

			var newBook = ScriptableUtils.CreateAsset<Book>(
				assetPath, "New Book");
			var varContainer = ScriptableUtils.AddToAsset<VariablesContainer>(
				newBook, ".settings.variables");

			newBook.bookName  = "New Book";
			newBook.variables = varContainer;

			AssetDatabase.SaveAssets();
		}

		[MenuItem("Assets/[SELF]/Create Chapter")]
		public static void CreateChapter()
		{
			var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

			var newChapter = ScriptableUtils.CreateAsset<Chapter>(
				assetPath, "New Chapter");

			// add start node
			var entryNode = ScriptableUtils.AddToAsset<EntryNode>(newChapter);

            entryNode.id = GUID.Generate().ToString();
            entryNode.UpdateName();
			// add exit node

			newChapter.chapterName = "New Chapter";

			AssetDatabase.SaveAssets();
		}

		//[MenuItem("StoryEditor/Fix Null Nodes")]
		//public static void FixNullNodes()
		//{
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
			//        var isSub = chapterAsset.nodeById.Any(n => (n as Node).behaviours.Contains(nb));

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
		//}

		//[MenuItem("StoryEditor/Fix Nodes Names")]
		//public static void FixNodesNames()
		//{
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
		//}

		//[MenuItem("StoryEditor/Check Asset Path")]
		//private static void CheckAssetPath()
		//{
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
		//}
	}
}