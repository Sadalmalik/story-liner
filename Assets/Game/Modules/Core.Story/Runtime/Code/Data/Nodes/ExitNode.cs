namespace Self.Story
{
	[NodeMetadata(customOutput: true)]
	public class ExitNode : BaseNode
	{
		// nextNodes must be empty all time
		public string tag;

		public string TargetNode;
	}
}