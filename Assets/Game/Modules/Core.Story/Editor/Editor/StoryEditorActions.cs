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
			var assetPath    = AssetDatabase.GetAssetPath(Selection.activeObject);

			var newChapter = ScriptableUtils.CreateAsset<Chapter>(
				assetPath, "New Chapter");
			
			newChapter.chapterName = "New Chapter";

			AssetDatabase.SaveAssets();
		}
	}
}