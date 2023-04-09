using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeExitController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ExitNode);

		[Inject] private StoryController _StoryController;
		
		public void Enter(
			BaseNode       node,
			Action<string> onNextCallback)
		{
			var exit = node as ExitNode;

			if (!string.IsNullOrEmpty(exit.TargetNode))
			{
				onNextCallback(exit.TargetNode);
			}
			else
			{
				_StoryController.ChapterComplete();
			}
		}

		public void Exit()
		{
		}
	}
}