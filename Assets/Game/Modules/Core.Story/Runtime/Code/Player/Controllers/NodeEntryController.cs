using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeEntryController : SharedObject, INodeController
	{
		public Type TargetType => typeof(EntryNode);

		[Inject] private StoryController _StoryController;

		public string Enter(BaseNode node)
		{
			if (node is EntryNode entry)
				return entry.NextNode;
			return node.NextNode;
		}
	}
}