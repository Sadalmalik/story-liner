using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeEntryController : NodeBaseController
	{
		public override Type GetTargetType() => typeof(EntryNode);
	}
}