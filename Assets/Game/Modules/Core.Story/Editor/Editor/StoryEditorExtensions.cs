using UnityEditor;

namespace Self.Story.Editors
{
	public static class StoryEditorExtensions
	{
		public static void UpdateName(this BaseNode node)
		{
			var croppedId = node.id.Substring(0, 8);
			var nodeType  = node.GetType().Name.Replace("Node", string.Empty);
			node.name = $"node.{nodeType}.{croppedId}";
			EditorUtility.SetDirty(node);
		}
	}
}