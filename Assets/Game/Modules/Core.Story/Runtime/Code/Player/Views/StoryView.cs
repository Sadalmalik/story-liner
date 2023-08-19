using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Self.Story
{
	public class StoryView : MonoBehaviour
	{
		[Header("General widgets")]
		public BackgroundWidget backgroundWidget;

		[Header("Story widgets")]
		public TextWidget textWidget;
		public CharacterWidget characterWidget;
		public ChoiceWidget choiceWidget;
		public CluesWidget cluesWidget;
		public DragItemsWidget dragItemsWidget;

		public GameObject animationsContainer;

		[Header("Story widgets")]
		public TMP_Text endMessage;

		public Button continueButton;

		public PlayableDirector[] animations { get; private set; }



		public void KeepViews(params AnimatedWidget[] views)
		{
			// huh?
			var animatedViews = new List<AnimatedWidget>();

			animatedViews.Add(textWidget);
			animatedViews.Add(characterWidget);
			animatedViews.Add(choiceWidget);
			animatedViews.Add(cluesWidget);

			foreach (var view in animatedViews)
			{
				if (!Array.Find(views, v => v.GetType() == view.GetType()))
				{
					view.Hide();
				}
			}
		}

		private void Awake()
		{
			animations = animationsContainer.GetComponentsInChildren<PlayableDirector>();
		}
	}
}
