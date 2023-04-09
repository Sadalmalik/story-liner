// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class RectTransformAnimator : AnimationNode
    {
        public float duration;
        [DrawWithUnityAttribute]
        public AnimationCurve curve = new AnimationCurve();

        [Space]
        public RectTransform target;
        public bool local;
        public RectTransformDataType type = RectTransformDataType.Position;

        [ShowIf("@DataTypesExtensions.IsVector1(type)")]
        public float floatStartValue;
        [ShowIf("@DataTypesExtensions.IsVector1(type)")]
        public float floatEndValue;
        
        [ShowIf("@DataTypesExtensions.IsVector2(type)")]
        public Vector2 vector2StartValue;
        [ShowIf("@DataTypesExtensions.IsVector2(type)")]
        public Vector2 vector2EndValue;
        
        [ShowIf("@DataTypesExtensions.IsVector3(type)")]
        public Vector3 vector3StartValue;
        [ShowIf("@DataTypesExtensions.IsVector3(type)")]
        public Vector3 vector3EndValue;

        public override void AppendTo(Sequence sequence)
        {
            if (type.IsVector3())
            {
                var assigner = TransformHelper.GetTransformVectorSetter((TransformDataType) type, target, local);
                
                float           value  = 0f;
                DOGetter<float> getter = () => value;
                DOSetter<float> setter = val =>
                {
                    value = val;
                    Vector3 evaluate = Vector3.LerpUnclamped(vector3StartValue, vector3EndValue, curve.Evaluate(val));
                    assigner(evaluate);
                };
                sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
            }
            else if (DataTypesExtensions.IsVector2(type))
            {
                var assigner = TransformHelper.GetRectTransformVector2Setter(type, target, local);
                
                float           value  = 0f;
                DOGetter<float> getter = () => value;
                DOSetter<float> setter = val =>
                {
                    value = val;
                    Vector3 evaluate = Vector3.LerpUnclamped(vector3StartValue, vector3EndValue, curve.Evaluate(val));
                    assigner(evaluate);
                };
                sequence.Insert(time, DOTween.To(getter, setter, 1, duration));
            }
            else
            {
                var assigner = TransformHelper.GetRectTransformFloatSetter(type, target, local);

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