using DG.Tweening;
using Kaleb.TweenAnimator;
using Self.Architecture.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Self.Story
{
	public class ChoiceWidget : ReplicaWidget
	{
		public ChoiceButtonWidget choicePrefab;
		public RectTransform      choiceContainer;

		public List<ChoiceButtonWidget> choices;

		public event Action<int> OnSelect;

		public new void Awake()
		{
			choices = new List<ChoiceButtonWidget>();

			for (int i = 0; i < 4; i++)
			{
				var widget = Instantiate(
					choicePrefab,
					choiceContainer,
					true);
				widget.OnClick += HandleSelect;
				choices.Add(widget);
			}
		}

		public void Show(ChoiceNode node)
		{
			base.Show(node as ReplicaNode);

			for (int i = 0; i < node.choices.Count; i++)
				choices[i].Init(i, node.choices[i].localizedText);
		}

		private void HandleSelect(int index)
		{
			OnSelect?.Invoke(index);
		}

		protected override void HandleCompleteHide()
		{
			foreach (var choice in choices)
				choice.gameObject.SetActive(false);
			base.HandleCompleteHide();
		}
	}
}