using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeChoiceController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ChoiceNode);

		[Inject] private StoryController _StoryController;

		public ChoiceNode Node { get; private set; }
		public TextWidget TextView { get; private set; }
		public ChoiceWidget ChoiceView { get; private set; }
		public CharacterWidget CharacterView { get; private set; }

		private int _selectedChoice;
		private bool _isShowing;
		private bool _isActive;



		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			signal.view.continueButton.onClick.AddListener(HandleClick);
			ChoiceView = signal.view.choiceWidget;
			TextView = signal.view.textWidget;
			CharacterView = signal.view.characterWidget;

			for (int i = 0; i < ChoiceView.choiceContainers.Count; i++)
			{
				var index = i;

				ChoiceView.choiceContainers[i].button.onClick.AddListener(() => HandleSelect(index));
			}
		}

		public string Enter(BaseNode node, BaseNode previousNode = null)
		{
			Node = node as ChoiceNode;
			TextView.InitText(Node.localized, HandleShowComplete);
			CharacterView.InitCharacter(Node.character);
			ChoiceView.InitChoices(Node.choices);

			_StoryController.StoryView.KeepViews(TextView, CharacterView, ChoiceView);

			ChoiceView.Show();
			CharacterView.Show();
			TextView.Show(typeof(ChoiceNode).Name);
			_isActive = true;

			return null;
		}

		private void HandleClick()
		{
			if (!_isActive)
				return;

			if (_isShowing)
			{
				TextView.SkipShowing();
				return;
			}
		}

		private void HandleSelect(int index)
		{
			if (!_isActive)
				return;

			if (_isShowing)
			{
				TextView.SkipShowing();
				return;
			}

			_isActive = false;
			_selectedChoice = index;
			_StoryController.SetNode(Node.nextNodes[_selectedChoice]);
		}

		private void HandleShowComplete()
		{
			_isShowing = false;
		}
	}
}
