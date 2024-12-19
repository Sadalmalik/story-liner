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
		public TweenAnimator showInstantAnimation;
		public TweenAnimator hideAnimation;

		[Space] public Button   button;
		public         TMP_Text text;
		public         float    textDuration;

		[Space] public RectTransform leftContainer;
		public         RectTransform rightContainer;
		public         RectTransform characterRoot;
		public         Image         characterImage;

		public event Action OnShowComplete;
		public event Action OnHideComplete;
		public event Action OnClick;

		protected ReplicaNode _replica;
        protected Tween       _tween;
        protected bool        _isHiding;
        protected bool        _isShowing;

		public void Awake()
		{
			if (button != null)
			{
				button.onClick.AddListener(HandleClick);
				button.interactable = false;
				// Внезапно мешает кликать по вышестоящим в иерархии объектам
				button.gameObject.SetActive(false);
			}
		}

		public virtual void Show(ReplicaNode node)
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
			showAnimation.Play();

			if (button != null)
			{
				button.interactable = true;
				button.gameObject.SetActive(true);
			}
		}

		protected void SetupCharacter()
		{
			var character = _replica.character;
			characterImage.sprite = character.characterIcon;

			characterRoot.gameObject.SetActive(characterImage.sprite != null);
			characterRoot.FitInside(character.isMainCharacter ? leftContainer : rightContainer);
		}

		public virtual void Hide()
		{
			_isHiding = true;
			hideAnimation.Play();
			hideAnimation.OnComplete += HandleCompleteHide;

			if (button != null)
			{
				button.interactable = false;
				button.gameObject.SetActive(false);
			}
		}

		protected virtual void HandleCompleteShow()
		{
			_isShowing = false;

			_tween?.Kill();
			_tween = null;
			text.SetText(_replica.localized);
			OnShowComplete?.Invoke();
		}

		protected virtual void HandleCompleteHide()
		{
			_isHiding = false;

			hideAnimation.OnComplete -= HandleCompleteHide;
			OnHideComplete?.Invoke();
		}

		private void HandleClick()
		{
			if (_isHiding)
			{
				return;
			}

			if (_isShowing)
			{
				_tween?.Kill();
				showAnimation.Stop();
				showInstantAnimation.Play();

				HandleCompleteShow();

				return;
			}

			OnClick?.Invoke();
		}
	}
}