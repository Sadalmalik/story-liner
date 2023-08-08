using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using UnityEngine;

namespace Self.Story
{
	public class NodeActiveController : NodeBaseController
	{
		public override Type GetTargetType() => typeof(ActiveNode);

		[Inject] private StoryController _StoryController;

		private ActiveNode       _node;
		private BackgroundWidget _background;
		private PauseWidget      _pause;

		private Sprite _oldBackground;

		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			_background = signal.view.backgroundWidget;
			_pause      = signal.view.pauseWidget;

			_pause.OnClick += HandleClick;
		}

		public override string Enter(BaseNode node)
		{
			_node = node as ActiveNode;

			foreach (var action in _node.actions)
			{
				if (action is PauseAction pauseAction)
				{
					_pause.Show();
					_oldBackground = _background.CurrentSprite;
					_background.SetImage(pauseAction.sprite);
					return null;
				}
			}

			return node.NextNode;
		}

		private void HandleClick()
		{
			_background.SetImage(_oldBackground);
			_StoryController.SetNode(_node.NextNode);
		}
	}
}