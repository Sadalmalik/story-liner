using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public interface INodeController
	{
		Type TargetType { get; }

		string Enter(BaseNode node);
	}
}