using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeChoiceController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ChoiceNode);

		[Inject] private StoryController _StoryController;

		public ChoiceNode    Node { get; private set; }
		public ChoiceWidget View { get; private set; }

		private int _selected;
		
		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			View = signal.view.choiceWidget;

			View.OnSelect       += HandleSelect;
			View.OnShowComplete += HandleShowComplete;
			View.OnHideComplete += HandleHideComplete;
		}

		public string Enter(BaseNode node)
		{
			Node = node as ChoiceNode;
			View.Show(Node);

			return null;
		}
		
		private void HandleSelect(int index)
		{
			_selected = index;
			View.Hide();
		}

		private void HandleShowComplete()
		{
			
		}
		
		private void HandleHideComplete()
		{
			_StoryController.SetNode(Node.nextNodes[_selected]);
		}
	}
}