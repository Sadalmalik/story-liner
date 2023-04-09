using UnityEngine;

namespace Self.Architecture.Utils
{
	public static class TransformExtensions
	{
		public static void FitInside(this RectTransform self, RectTransform target)
		{
			self.SetParent(target);
			self.anchorMin     = Vector2.zero;
			self.anchorMax     = Vector2.one;
			self.pivot         = Vector2.one * 0.5f;
			self.sizeDelta     = Vector2.zero;
			self.localPosition = Vector2.zero;
		}
	}
}