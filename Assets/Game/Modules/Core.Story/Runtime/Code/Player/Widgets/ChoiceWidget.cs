using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
    // Container to display player choices
	public class ChoiceWidget : AnimatedWidget
	{
		[Space]
		public ChoiceButtonWidget choicePrefab;
		public RectTransform      choiceContainer;

		public List<ChoiceButtonWidget> choiceContainers;



        public void InitChoices(List<ChoiceNode.Choice> choices)
        {
            for (int i = 0; i < choiceContainers.Count; i++)
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
