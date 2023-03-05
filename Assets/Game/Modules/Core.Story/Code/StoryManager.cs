using UnityEngine;

public class StoryManager
{
	private Chapter _chapter;

	private string _currentNode;

	public void SetChapter(Chapter chapter)
	{
		
	}

	public void SetNode(string nodeId)
	{
		if (!_chapter.nodes.TryGetValue(nodeId, out var node))
		{
			Debug.Log("AAA!!!");
			return;
		}
	}
}