using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public interface INodeController
	{
		Type GetTargetType();

		string Enter(BaseNode node);
	}
}