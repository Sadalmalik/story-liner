using System;
using UnityEngine;
using UnityEngine.UI;

namespace Self.General.UI
{
	[ExecuteInEditMode]
	public class ProgressBarWidget : MonoBehaviour
	{
		public Image fill;

		[SerializeField, Range(0, 1)]
		private float _progress;

		public float Progress => _progress;

		public void SetProgress(float progress)
		{
			_progress       = progress;
			fill.fillAmount = progress;
		}

#if UNITY_EDITOR
		public void Update()
		{
			fill.fillAmount = _progress;
		}
#endif
	}
}