// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using System.Collections.Generic;

namespace UnityEngine.UI
{
	[ExecuteInEditMode]
	[AddComponentMenu("Layout/Circle Layout Group", 153)]
	public class UICircleLayoutGroup : MonoBehaviour
	{
		public float radius = 1;

		[Range(0f, 360f)]
		public float startAngle = 0;

		[Range(0f, 5f)]
		public float scale = 1;

		public bool clockwise;
		public bool useChilds;
		public RectTransform[] targets;

		[Space]
		public bool forceRecalculate;

		public bool keepRecalculate;

		public void Awake()
		{
			Recalculate();
		}

		public void Update()
		{
			if (forceRecalculate)
			{
				if (Application.isEditor && !Application.isPlaying)
					forceRecalculate = keepRecalculate;
				Recalculate();
			}
		}

		private List<RectTransform> _rects;

		public void SetAngle(float angle)
		{
			startAngle = angle;
			Recalculate();
		}

		public void SetRadius(float radius)
		{
			this.radius = radius;
			Recalculate();
		}

		public void SetScale(float scale)
		{
			this.scale = scale;
			Recalculate();
		}

		private void GetActiveRects()
		{
			if (_rects == null)
				_rects = new List<RectTransform>();
			_rects.Clear();

			if (useChilds)
			{
				foreach (RectTransform rt in transform)
					if (rt.gameObject.activeSelf)
						_rects.Add(rt);
			}
			else
			{
				if (targets != null && targets.Length > 0)
				{
					foreach (RectTransform rt in targets)
						if (rt.gameObject.activeSelf)
							_rects.Add(rt);
				}
			}
		}

		public void Recalculate()
		{
			GetActiveRects();

			var count     = _rects.Count;
			var direction = clockwise ? -1f : 1f;
			var step      = 360f / count;

			for (var i = 0; i < count; i++)
			{
				var rect       = _rects[i];
				var finalAngle = startAngle + direction * i * step;
				rect.localPosition = radius * AngleToDirection(finalAngle);
				rect.localScale    = Vector3.one * scale;
			}
		}

		public Vector2 AngleToDirection(float angle)
		{
			angle *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}
	}
}