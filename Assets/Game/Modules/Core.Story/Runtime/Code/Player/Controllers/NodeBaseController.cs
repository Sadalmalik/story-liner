using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public abstract class NodeBaseController : SharedObject
	{
		public abstract Type TargetType { get; }
		
		public abstract void Enter(BaseNode node, Action<string> onNext);
		
		public abstract void Exit();
	}
}