using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class BackgroundWidget : MonoBehaviour
	{
		public Image backA;
		public Image backB;

		public float duration = 0.6f;

		private Sequence _sequence;
		
		public void Awake()
		{
			backA.color = Color.white;
			backB.color = Color.clear;
		}

		public void SetImage(Sprite sprite, bool instantly=false)
		{
			_sequence?.Kill();

			if (instantly)
			{
				backA.sprite = sprite;
				backA.color  = Color.white;
				backB.color  = Color.clear;
			}
			else
			{
				_sequence = DOTween.Sequence();
				_sequence.AppendCallback(() => backB.sprite = sprite);
				_sequence.Append(backB.DOFade(1, duration));
				_sequence.AppendCallback(() =>
				{
					backA.sprite = sprite;
					backB.color  = Color.clear;
				});
			}
		}
	}
}