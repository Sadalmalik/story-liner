using Self.Architecture.IOC;

namespace Self.Story
{
	public class SetCluesAction : BaseAction
	{
		[DisplayOnNode(1)] public ClueData[] clues;

		[Inject] private StoryController _StoryController;

		public override void Execute(BaseNode node)
		{
			var cluesWidget = _StoryController.StoryView.cluesWidget;

			cluesWidget.SetClues(clues);
		}
	}
}