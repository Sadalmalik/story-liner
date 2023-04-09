using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ReplicaNode);

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