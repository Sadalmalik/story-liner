using System;

namespace Self.Story
{
	public abstract class NodeBaseController
	{
		public abstract void Enter(BaseNode node, Action<string> onNext);
		
		public abstract void Exit();
	}
}