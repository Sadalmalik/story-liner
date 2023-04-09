using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeExitController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ExitNode);

		public void Enter(
			BaseNode       node,
			Action<string> onNextCallback)
		{
			var exit = node as ExitNode;
			
			onNextCallback(exit.TargetNode);
		}

		public void Exit()
		{
		}
	}
}