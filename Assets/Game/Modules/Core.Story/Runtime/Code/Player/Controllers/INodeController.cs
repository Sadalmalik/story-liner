using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public interface INodeController : IShared
	{
		Type TargetType { get; }

		string Enter(BaseNode node);
	}
}