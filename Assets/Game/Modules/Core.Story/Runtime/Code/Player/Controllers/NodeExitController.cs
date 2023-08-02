using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeExitController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ExitNode);

		[Inject] private StoryController _StoryController;

        public string Enter(BaseNode node, BaseNode previousNode = null)
        {
			var exit = node as ExitNode;

			if (!string.IsNullOrEmpty(exit.TargetNode))
			{
				return exit.TargetNode;
			}
			else
			{
				//_StoryController.ChapterComplete();
			}

			return exit.NextNode;
		}
	}
}