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
		public TweenAnimator showAnimation;
		public TweenAnimator hideAnimation;

		[Space]
		public Button button;
		public TMP_Text text;
		public float    textDuration;

		[Space]
		public RectTransform leftContainer;
		public RectTransform rightContainer;
		public RectTransform characterRoot;
		public Image         characterImage;

		public event Action OnShowComplete;
		public event Action OnHideComplete;
		public event Action OnClick;

		private ReplicaNode _replica;
		private Tween       _tween;

		public void Awake()
		{
			button.onClick.AddListener(HandleClick);
		}

		public virtual void Show(ReplicaNode node)
		{
			_replica = node;
			SetupCharacter();
			text.SetText(string.Empty);
			_tween = DOTween.Sequence()
				.Join(text.DOText(_replica.localized, textDuration))
				.InsertCallback(showAnimation.Duration, HandleCompleteShow);
			_tween.Play();
			showAnimation.Play();
		}

		private void SetupCharacter()
		{
			var character = _replica.character;
			characterImage.sprite = character.characterIcon;
			
			characterRoot.gameObject.SetActive(characterImage.sprite!=null);
			characterRoot.FitInside(character.isMainCharacter ? leftContainer : rightContainer);
		}

		public virtual void Hide()
		{
			hideAnimation.Play();
			hideAnimation.OnComplete += HandleCompleteHide;
		}

		protected virtual void HandleCompleteShow()
		{
			_tween = null;
			OnShowComplete?.Invoke();
		}

		protected virtual void HandleCompleteHide()
		{
			hideAnimation.OnComplete -= HandleCompleteHide;
			OnHideComplete?.Invoke();
		}

		private void HandleClick()
		{
			if (_tween != null)
			{
				_tween?.Kill();
				_tween = null;
				
				showAnimation.Stop();

				text.SetText(_replica.localized);
			}
			else
			{
				OnClick?.Invoke();
			}
		}
	}
}