using DG.Tweening;
using Kaleb.TweenAnimator;
using Self.Architecture.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Self.Story
{
	public class ChoiceWidget : ReplicaWidget
	{
		[Space]
		public ChoiceButtonWidget choicePrefab;
		public RectTransform      choiceContainer;

		public List<ChoiceButtonWidget> choices;

		public event Action<int> OnSelect;

		public new void Awake()
		{
			//choices = new List<ChoiceButtonWidget>();
		
			for (int i = 0; i < 4; i++)
			{
				var widget = choices[i];
				widget.OnClick += HandleSelect;
			}
		}

		public void Show(ChoiceNode node)
		{
			base.Show(node as ReplicaNode);

			for (int i = 0; i < choices.Count; i++)
			{
				var choice  = choices[i];
				var enabled = i < node.choices.Count;
				choice.gameObject.SetActive(enabled);
				if (enabled)
					choice.Init(i, node.choices[i].localizedText);
			}
			
			LayoutRebuilder.ForceRebuildLayoutImmediate(choiceContainer);
		}

		private void HandleSelect(int index)
		{
			OnSelect?.Invoke(index);
		}

		protected override void HandleCompleteHide(PlayableDirector hideAnim)
		{
			foreach (var choice in choices)
				choice.Hide();

			base.HandleCompleteHide(hideAnim);
		}
	}
}
