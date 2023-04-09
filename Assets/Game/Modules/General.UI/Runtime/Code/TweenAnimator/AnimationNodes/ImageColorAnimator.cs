// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Kaleb.TweenAnimator
{
	public class ImageColorAnimator : AnimationNode
	{
		public float duration;
        [DrawWithUnityAttribute]
		public AnimationCurve curve = new AnimationCurve();

		[Space]
		public Image target;
		public ColorDataType type = ColorDataType.Color;

		[Space]
        [ShowIf("@!DataTypesExtensions.IsColor(type)")]
		public float floatStartValue;
        [ShowIf("@!DataTypesExtensions.IsColor(type)")]
		public float floatEndValue;
        [ShowIf("@DataTypesExtensions.IsColor(type)")]
		public Color colorStartValue;
        [ShowIf("@DataTypesExtensions.IsColor(type)")]
		public Color colorEndValue;

		public override void AppendTo(Sequence sequence)
		{
			if (type.IsColor())
			{
				var             assigner = ColorHelper.GetImageColorSetter(type, target);
				float           value    = 0f;
				DOGetter<float> getter   = () => value;
				DOSetter<float> setter = val =>
				{
					value = val;
					Color evaluate = Color.LerpUnclamped(colorStartValue, colorEndValue, curve.Evaluate(val));
					assigner(evaluate);
				};
				sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
			}
			else
			{
				var             assigner = ColorHelper.GetImageFloatSetter(type, target);
				float           value    = 0f;
				DOGetter<float> getter   = () => value;
				DOSetter<float> setter = val =>
				{
					value = val;
					float evaluate = Mathf.LerpUnclamped(floatStartValue, floatEndValue, curve.Evaluate(val));
					assigner(evaluate);
				};
				sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
			}
		}
    }
}