using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeChapterController : SharedObject, INodeController
	{
		public Type   GetTargetType() => typeof(ChapterNode);

		[Inject] private StoryController _storyController;

        public string Enter(BaseNode node, BaseNode previousNode = null)
        {
			var chapterNode = node as ChapterNode;
			
			_storyController.SetChapter(chapterNode.chapter, null);

			return null;
		}
	}
}