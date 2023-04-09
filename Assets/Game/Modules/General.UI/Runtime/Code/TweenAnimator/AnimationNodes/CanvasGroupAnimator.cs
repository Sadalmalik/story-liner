using DG.Tweening;
// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class CanvasGroupAnimator : AnimationNode
    {
        public float duration;
        [DrawWithUnityAttribute]
        public AnimationCurve curve = new AnimationCurve();

        [Space]
        public CanvasGroup target;

        [Space]
        public float floatStartValue;
        public float floatEndValue;

        public override void AppendTo(Sequence sequence)
        {
            float           value  = 0f;
            DOGetter<float> getter = () => value;
            DOSetter<float> setter = val =>
            {
                value = val;
                float evaluate = Mathf.LerpUnclamped(floatStartValue, floatEndValue, curve.Evaluate(val));
                target.alpha = evaluate;
            };
            sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
        }
    }
}