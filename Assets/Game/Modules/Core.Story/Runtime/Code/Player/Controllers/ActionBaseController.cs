using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public abstract class ActionBaseController : SharedObject
	{
		public abstract Type TargetType { get; }
		
		public abstract void Execute(BaseNode node);
	}
}