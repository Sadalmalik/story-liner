using System;
using DG.Tweening;
using Self.Architecture.Utils;
using TMPro;
using UnityEngine.UI;

namespace Self.Story
{
	public class ReplicaWidget : StoryNodeWidget
	{
		public Button   button;
		public TMP_Text text;
		public float    duration;

		private ReplicaNode _replica;

		private Tween _tween;

		public void Awake()
		{
			button.onClick.AddListener(OnClick);
		}

		public override void SetNode(BaseNode node)
		{
			_replica = node as ReplicaNode;

			_tween = text
				.DOText(_replica.localized, duration)
				.OnComplete(HandCompleteTextAnimation);
		}

		private void HandCompleteTextAnimation()
		{
			_tween?.Kill();
			_tween = null;
		}

		private void OnClick()
		{
			if (_tween != null)
			{
				_tween?.Kill();
				_tween = null;

				text.SetText(_replica.localized);
			}
			else
			{
				InvokeNext(_replica.NextNode);
			}
		}
	}
}