using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeChoiceController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ReplicaNode);

		public string Enter(
			BaseNode       node)
		{
			return node.nextNodes[0];
		}

		public void Exit()
		{
		}
	}
}