using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public interface INodeController : IShared
	{
		Type TargetType { get; }

		void Enter(BaseNode node, Action<string> onNext);

		void Exit();
	}
}