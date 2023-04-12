﻿using System;
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

		[Space]
		public Button   button;
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
		private bool        _isHiding;
		private bool        _isShowing;

		public void Awake()
		{
			button.onClick.AddListener(HandleClick);
		}

		public virtual void Show(ReplicaNode node)
		{
			_replica   = node;
			_isShowing = true;
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

			characterRoot.gameObject.SetActive(characterImage.sprite != null);
			characterRoot.FitInside(character.isMainCharacter ? leftContainer : rightContainer);
		}

		public virtual void Hide()
		{
			_isHiding = true;
			hideAnimation.Play();
			hideAnimation.OnComplete += HandleCompleteHide;
		}

		protected virtual void HandleCompleteShow()
		{
			_isShowing = false;
			
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