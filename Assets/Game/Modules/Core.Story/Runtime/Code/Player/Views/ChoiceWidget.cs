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

		public List<ChoiceButtonWidget> choiceContainers;



        public void InitChoices(List<ChoiceNode.Choice> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choiceContainers[i];
                var enabled = i < choices.Count;
                choice.gameObject.SetActive(enabled);

                if (enabled)
                    choice.Init(i, choices[i].localizedText);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(choiceContainer);
        }
    }
}
