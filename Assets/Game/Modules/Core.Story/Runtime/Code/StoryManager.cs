using Self.Architecture.IOC;
using UnityEngine;

namespace Self.Story
{
	public class StoryManager : SharedObject
	{
		private Chapter _chapter;

		private string _currentNode;

		public override void Init()
		{
			Debug.Log("[TEST] StoryManager.Init()");
		}

		public override void Dispose()
		{
			Debug.Log("[TEST] StoryManager.Dispose()");
		}
		
		public void SetChapter(Chapter chapter)
		{
		
		}

		public void SetNode(string nodeId)
		{
			//if (!_chapter.nodesByID.TryGetValue(nodeId, out var node))
			//{
			//	Debug.Log("AAA!!!");
			//	return;
			//}
		}
	}
}