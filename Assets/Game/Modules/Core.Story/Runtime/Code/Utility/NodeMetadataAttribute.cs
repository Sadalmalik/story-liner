using System;

namespace Self.Story
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeMetadataAttribute : Attribute
	{
		public readonly bool customInput;
		public readonly bool customOutput;

		public NodeMetadataAttribute(bool customInput = false, bool customOutput = false)
		{
			this.customInput  = customInput;
			this.customOutput = customOutput;
		}
	}
}