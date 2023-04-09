// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Kaleb.TweenAnimator
{
    public class RangedCurve
    {
        public bool enabled;
        [DrawWithUnityAttribute]
        public AnimationCurve curve = new AnimationCurve();
        public float floatStartValue;
        public float floatEndValue;

        public float Evaluate(float value)
        {
            return Mathf.LerpUnclamped(floatStartValue, floatEndValue, curve.Evaluate(value));
        }
    }

    public class CircleLayoutAnimator : AnimationNode
    {
        public float duration;
        [Space]
        public UICircleLayoutGroup circleGroup;
        [Space]
        public RangedCurve radiusCurve;
        public RangedCurve angleCurve;
        public RangedCurve scaleCurve;
        
        public override void AppendTo(Sequence sequence)
        {
            float value = 0;
            sequence.Insert(time, DOTween.To(
                () => value,
                val =>
                {
                    value = val;
                    bool changed = false;
                    if (radiusCurve!=null && radiusCurve.enabled)
                    {
                        changed = true;
                        circleGroup.radius = radiusCurve.Evaluate(val);
                    }
                    if (angleCurve!=null && angleCurve.enabled)
                    {
                        changed = true;
                        circleGroup.startAngle = angleCurve.Evaluate(val);
                    }
                    if (scaleCurve!=null && scaleCurve.enabled)
                    {
                        changed = true;
                        circleGroup.scale = scaleCurve.Evaluate(val);
                    }
                    if (changed)
                        circleGroup.Recalculate();
                },
                1, duration
                ));
        }
    }
}

//*/
