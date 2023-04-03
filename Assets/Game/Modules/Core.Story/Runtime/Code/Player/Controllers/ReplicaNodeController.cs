﻿using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class ReplicaNodeController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ReplicaNode);

		public void Enter(
			BaseNode       node,
			Action<string> onNext)
		{
			onNext(node.nextNodes[0]);
		}

		public void Exit()
		{
		}
	}
}