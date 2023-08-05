using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ReplicaNode);

		[Inject] private StoryController _StoryController;

		public ReplicaNode Node { get; private set; }
		public CharacterWidget CharacterView { get; private set; }
		public TextWidget TextView { get; private set; }

		private bool _isShowing;
		private bool _isActive;



		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			signal.view.continueButton.onClick.AddListener(HandleClick);
			TextView = signal.view.textWidget;
			CharacterView = signal.view.characterWidget;
		}

		public string Enter(BaseNode node, BaseNode previousNode = null)
		{
			Node = node as ReplicaNode;

			TextView.InitText(Node.localized, HandleShowComplete);
			CharacterView.InitCharacter(Node.character);
			_isShowing = true;

			_StoryController.StoryView.KeepViews(TextView, CharacterView);

			CharacterView.Show();
			TextView.Show(typeof(ReplicaNode).Name);
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

			_isActive = false;
			_StoryController.SetNode(Node.NextNode);
		}

		private void HandleShowComplete()
		{
			_isShowing = false;
		}
	}
}
