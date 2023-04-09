using System;
using DG.Tweening;
using Kaleb.TweenAnimator;
using Self.Architecture.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class ReplicaWidget : MonoBehaviour
	{
		public Button   button;
		public TMP_Text text;
		public float    textDuration;

		[Space]
		public TweenAnimator showAnimation;
		public TweenAnimator hideAnimation;

		public event Action OnShowComplete;
		public event Action OnHideComplete;
		public event Action OnClick;

		private ReplicaNode _replica;
		private Tween       _tween;

		public void Awake()
		{
			button.onClick.AddListener(HandleClick);
		}

		public void Show(ReplicaNode node)
		{
			_replica = node;
			_tween = DOTween
				.Sequence()
				.Join(text.DOText(_replica.localized, textDuration))
				.Join(showAnimation.sequence)
				.AppendCallback(HandleCompleteShow);
			_tween.Play();
		}

		public void Hide()
		{
			_tween = DOTween
				.Sequence()
				.Join(hideAnimation.sequence)
				.AppendCallback(HandleCompleteHide);
			_tween.Play();
		}

		private void HandleCompleteShow()
		{
			OnShowComplete?.Invoke();
		}

		private void HandleCompleteHide()
		{
			OnHideComplete?.Invoke();
		}

		private void HandleClick()
		{
			if (_tween != null)
			{
				_tween?.Kill();
				_tween = null;

				text.SetText(_replica.localized);
			}
			else
			{
				OnClick?.Invoke();
			}
		}
	}
}