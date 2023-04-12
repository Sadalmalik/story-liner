using System;

namespace Self.Story
{
	public class NodeActiveController : NodeBaseController
	{
		public override Type GetTargetType() => typeof(ActiveNode);
	}
}