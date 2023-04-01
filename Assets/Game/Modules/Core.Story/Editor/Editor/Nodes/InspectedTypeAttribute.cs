using System;

namespace Self.Story.Editors
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InspectedTypeAttribute : Attribute
	{
		public readonly Type inspectedType;

		public InspectedTypeAttribute(Type inspectedType) => this.inspectedType = inspectedType;
	}
}