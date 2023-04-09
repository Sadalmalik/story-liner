using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class TextColorAnimator : AnimationNode
    {
        public float duration;
        [DrawWithUnity]
        public AnimationCurve curve = new AnimationCurve();

        [Space]
        public TMP_Text target;
        public ColorDataType type = ColorDataType.Color;
		
        [Space]
        [ShowIf("@!DataTypesExtensions.IsColor(type)")]
        public float floatStartValue;
        [ShowIf("@!DataTypesExtensions.IsColor(type)")]
        public float floatEndValue;
		
        public override void AppendTo(Sequence sequence)
        {
            var             assigner = ColorHelper.GetTMPTextFloatSetter(type, target);
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