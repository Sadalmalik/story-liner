using System;
using DG.Tweening;
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
		public float    duration;

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
			_tween = text
				.DOText(_replica.localized, duration)
				.OnComplete(HandCompleteTextAnimation);
		}

		public void Hide()
		{
			
		}

		private void HandCompleteTextAnimation()
		{
			_tween?.Kill();
			_tween = null;
			
			OnShowComplete?.Invoke();
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