using DG.Tweening;
using TMPro;

namespace Self.Architecture.Utils
{
	public static class DOTweenEx
	{
		public static Tweener DOText(
			this TextMeshProUGUI target,
			string               endValue,
			float                duration,
			bool                 richTextEnabled = true,
			ScrambleMode         scrambleMode    = ScrambleMode.None,
			string               scrambleChars   = null)
		{
			return DOTween.To(
					() => target.text,
					x => target.text = x,
					endValue, duration)
				.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
				.SetEase(Ease.Linear)
				.SetTarget(target);
		}

		public static Tweener DOText(
			this TMP_Text target,
			string        endValue,
			float         duration,
			bool          richTextEnabled = true,
			ScrambleMode  scrambleMode    = ScrambleMode.None,
			string        scrambleChars   = null)
		{
			return DOTween.To(
					() => target.text,
					x => target.text = x,
					endValue, duration)
				.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
				.SetEase(Ease.Linear)
				.SetTarget(target);
		}
	}
}