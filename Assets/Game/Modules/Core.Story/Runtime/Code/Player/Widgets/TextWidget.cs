using System;
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
		public event Action<string> onTextChanged;

		public TMP_Text TextContainer;

		[SerializeField] float textDurationPerCharacter;

		private Tween _tween;
		private TweenCallback _callback;
		private string _text;



		public void InitText(string text, TweenCallback callback)
		{
			_callback = callback;
			_text = text;
			TextContainer.text = string.Empty;
			onTextChanged?.Invoke(null);

			var duration = text.Length * textDurationPerCharacter;

			_tween?.Kill();
			_tween = DOTween.Sequence()
				.Join(TextContainer.DOText(text, duration))
				.InsertCallback(duration, callback)
				.InsertCallback(duration, () =>
				{
					GetTags();
				});
			_tween.Play();
		}

		public void SkipShowing()
		{
			_tween?.Kill();
			_tween = null;
			TextContainer.text = _text;
			_callback?.Invoke();
			onTextChanged?.Invoke(_text);
		}

		private void GetTags()
		{
			//var dataList = new List<string>();
			//var meta = TextContainer.text.Split("link=\"");
			//var offset = "data=".Length;
			//var c = TextContainer.textInfo;
			//var cc = c.characterInfo;
			//TextContainer.material

			//foreach (var m in meta)
			//{
			//	var length = m.IndexOf("\"") - offset;

			//	if (length < 0)
			//		continue;

			//	var dataRead = m.Substring(offset, length);
			//	dataList.Add(dataRead);
			//}

			onTextChanged?.Invoke(_text);
		}
	}
}
