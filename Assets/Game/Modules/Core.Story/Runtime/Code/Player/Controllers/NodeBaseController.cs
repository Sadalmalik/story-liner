﻿using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeBaseController : SharedObject, INodeController
	{
		public virtual Type GetTargetType() => typeof(BaseNode);

		public string Enter(BaseNode node)
		{
			return node.NextNode;
		}
	}
}