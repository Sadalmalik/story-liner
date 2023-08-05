using System.Linq;
using DG.Tweening;
using Self.Architecture.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Self.Story
{
    // Container to display text
    public class TextWidget : AnimatedWidget
    {
        public TMP_Text TextContainer;

		[SerializeField] float textDuration;

        private Tween _tween;
		private TweenCallback _callback;
		private string _text;



        public void InitText(string text, TweenCallback callback)
        {
			_callback = callback;
			_text = text;
			TextContainer.text = string.Empty;

			_tween?.Kill();
            _tween = DOTween.Sequence()
                .Join(TextContainer.DOText(text, textDuration))
                .InsertCallback(textDuration, callback);
            _tween.Play();
        }

		public void SkipShowing()
		{
			_tween?.Kill();
			_tween = null;
			TextContainer.text = _text;
			_callback?.Invoke();
		}
	}
}
