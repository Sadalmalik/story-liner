using System;

namespace Self.Story
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisplayOnNodeAttribute : Attribute
	{
		/// <summary>Ordering starts from top</summary>
		public readonly int order;

		public DisplayOnNodeAttribute(int order = 0)
		{
			this.order = order;
		}
	}
}