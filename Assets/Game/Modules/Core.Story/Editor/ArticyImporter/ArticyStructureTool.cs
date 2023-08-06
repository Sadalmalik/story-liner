using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Self.ArticyImporter
{
	public static class ArticyStructureTool
	{
		[MenuItem("StoryEditor/Reorder folders")]
		private static void ReorderFolders()
		{
			var directory = Path.GetFullPath(
				AssetDatabase.GetAssetPath(Selection.activeObject)
			).Replace("\\", "/");

			if (!Directory.Exists(directory))
				directory = Path.GetDirectoryName(directory);

			if (!directory.EndsWith("Assets"))
				throw new Exception("Articy folder must be Assets!");

			ReorderArticyFolders(directory);
		}

		public static void ReorderArticyFolders(string directory)
		{
			var pathCharacters = Path.Combine(directory, "Assets/Изображения/Действующие_лица");
			if (Directory.Exists(pathCharacters))
			{
				var newPathCharacters = Path.Combine(directory, "Characters");
				Debug.Log($"Move: {pathCharacters} -> {newPathCharacters}");
				Directory.Move(pathCharacters, newPathCharacters);
			}

			var pathLocations = Path.Combine(directory, "Assets/Изображения/Локации");
			if (Directory.Exists(pathLocations))
			{
				var newPathLocations = Path.Combine(directory, "Locations");
				Debug.Log($"Move: {pathLocations} -> {newPathLocations}");
				Directory.Move(pathLocations, newPathLocations);
			}
			
			AssetDatabase.Refresh();
		}
	}
}