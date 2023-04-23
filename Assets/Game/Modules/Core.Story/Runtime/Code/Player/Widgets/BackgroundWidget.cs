using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class BackgroundWidget : MonoBehaviour
	{
		public Image background;
		public Image fade;

		public float duration = 0.6f;

		private Sequence _sequence;
		
		public void Awake()
		{
			background.color = Color.white;
			fade.color       = Color.clear;
		}

		public void SetImage(Sprite sprite, bool instantly=false)
		{
			_sequence?.Kill();

			if (instantly)
			{
				background.sprite = sprite;
				fade.color        = Color.clear;
			}
			else
			{
				var halfDuration = duration * 0.5f;
				_sequence = DOTween.Sequence();
				_sequence.Append(fade.DOFade(1, halfDuration));
				_sequence.AppendCallback(() => background.sprite = sprite);
				_sequence.Append(fade.DOFade(0, halfDuration));
			}
		}
	}
}