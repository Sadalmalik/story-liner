using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeBaseController : SharedObject, INodeController
	{
		public Type TargetType => typeof(BaseNode);

		public void Enter(
			BaseNode       node,
			Action<string> onNextCallback)
		{
			onNextCallback(node.nextNodes[0]);
		}

		public void Exit()
		{
		}
	}
}