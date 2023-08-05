using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

		[Header("Story widgets")]
		public TMP_Text endMessage;

		public Button continueButton;



		public void KeepViews(params AnimatedWidget[] views)
		{
			// huh?
			var animatedViews = new List<AnimatedWidget>();

			animatedViews.Add(textWidget);
			animatedViews.Add(characterWidget);
			animatedViews.Add(choiceWidget);

			foreach (var view in animatedViews)
			{
				if (!Array.Find(views, v => v.GetType() == view.GetType()))
				{
					view.Hide();
				}
			}
		}
	}
}
