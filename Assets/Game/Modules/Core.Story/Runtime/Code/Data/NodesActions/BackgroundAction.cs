using System;
using Self.Architecture.IOC;
using UnityEngine;

namespace Self.Story
{
	public class BackgroundAction : BaseAction
	{
		[DisplayOnNode(1)] public Sprite sprite;

		[Inject] private StoryController _storyController;
		
		public override void Execute(BaseNode node)
		{
			_storyController.StoryView.backgroundWidget.SetImage(sprite);
		}
	}
}