namespace Self.Story
{
	public class ChoiceWidget : StoryNodeWidget
	{
		public override void SetNode(BaseNode node)
		{
			var choice = node as ChoiceNode;
		}
	}
}