using System;

namespace Self.Story
{
	public abstract class ActionBaseController
	{
		public abstract void Execute(BaseAction node, Action<string> onNext);
	}
}