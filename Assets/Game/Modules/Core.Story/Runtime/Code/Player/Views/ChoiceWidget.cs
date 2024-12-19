using DG.Tweening;
using Kaleb.TweenAnimator;
using Self.Architecture.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class ChoiceWidget : ReplicaWidget
	{
		public TweenAnimator showAnimationMultipleChoice;
		public TweenAnimator hideAnimationMultipleChoice;
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
			_replica   = node;
			_isShowing = true;
			SetupCharacter();
			_tween?.Kill();
			text.SetText(string.Empty);
			_tween = DOTween.Sequence()
				.Join(text.DOText(_replica.localized, textDuration))
				.InsertCallback(showAnimation.Duration, HandleCompleteShow);
			_tween.Play();

            if (node.choices.Count > 2)
                showAnimationMultipleChoice.Play();
            else
                showAnimation.Play();

			if (button != null)
			{
				button.interactable = true;
				button.gameObject.SetActive(true);
			}

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

        public override void Hide() 
        {
            _isHiding = true;

            var choiceNode = _replica as ChoiceNode;

            if (choiceNode.choices.Count > 2) 
            {
                hideAnimationMultipleChoice.Play();
                hideAnimationMultipleChoice.OnComplete += HandleCompleteHide;
            }
            else 
            {
                hideAnimation.Play();
                hideAnimation.OnComplete += HandleCompleteHide;
            }

            if (button != null) {
                button.interactable = false;
                button.gameObject.SetActive(false);
            }
        }

		private void HandleSelect(int index)
		{
			OnSelect?.Invoke(index);
		}

		protected override void HandleCompleteHide()
		{
			foreach (var choice in choices)
				choice.Hide();
			base.HandleCompleteHide();
		}
	}
}