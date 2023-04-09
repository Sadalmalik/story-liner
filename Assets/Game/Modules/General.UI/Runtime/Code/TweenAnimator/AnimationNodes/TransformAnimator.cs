// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
	public enum TransformAnimatorMode
	{
		Values,
		Anchors,
	}
	
	public class TransformAnimator : AnimationNode
	{
		public float duration;
        [DrawWithUnityAttribute]
		public AnimationCurve curve = new AnimationCurve();

		[Space]
		public Transform target;
		public TransformDataType type = TransformDataType.Position;
		public bool local;

		[Space]
		public TransformAnimatorMode mode = TransformAnimatorMode.Values;
		public bool startFromSelf;
        [ShowIf("@mode != TransformAnimatorMode.Anchors && DataTypesExtensions.IsVector1(type)"), DisableIf("startFromSelf")]
		public float floatStartValue;
        [ShowIf("@mode != TransformAnimatorMode.Anchors && DataTypesExtensions.IsVector1(type)")]
		public float floatEndValue;
        [ShowIf("@mode != TransformAnimatorMode.Anchors && DataTypesExtensions.IsVector3(type)"), DisableIf("startFromSelf")]
		public Vector3 vectorStartValue;
        [ShowIf("@mode != TransformAnimatorMode.Anchors && DataTypesExtensions.IsVector3(type)")]
		public Vector3 vectorEndValue;
        [ShowIf("@mode == TransformAnimatorMode.Anchors"), DisableIf("startFromSelf")]
		public Transform startAnchor;
        [ShowIf("@mode == TransformAnimatorMode.Anchors")]
		public Transform endAnchor;

		public override void AppendTo(Sequence sequence)
		{
			if (type.IsVector3())
			{
				var useAnchors = mode == TransformAnimatorMode.Anchors;
				var assigner = TransformHelper.GetTransformVectorSetter(type, target, local & !useAnchors);
				
				if (useAnchors)
				{
					vectorStartValue = startAnchor.position;
					vectorEndValue   = endAnchor.position;
				}

				if (startFromSelf)
				{
					vectorStartValue = TransformHelper.GetTransformVectorGetter(type, target, local)();
				}

				float           value  = 0f;
				DOGetter<float> getter = () => value;
				DOSetter<float> setter = val =>
				{
					value = val;
					Vector3 evaluate = Vector3.LerpUnclamped(vectorStartValue, vectorEndValue, curve.Evaluate(val));
					assigner(evaluate);
				};
				sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
			}
			else
			{
				var assigner = TransformHelper.GetTransformFloatSetter(type, target, local);

				if (startFromSelf)
				{
					floatStartValue = TransformHelper.GetTransformFloatGetter(type, target, local)();
				}
				
				float           value  = 0f;
				DOGetter<float> getter = () => value;
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