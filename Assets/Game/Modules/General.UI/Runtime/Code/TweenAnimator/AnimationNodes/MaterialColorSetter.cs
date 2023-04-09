// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Kaleb.TweenAnimator
{
    public class MaterialColorSetter : AnimationNode
    {
        [Space]
        public Renderer target;
        public bool useSharedMaterial;
        public ColorDataType type = ColorDataType.Color;

        [Space]
        [ShowIf("@!ColorHelper.IsColor(type)")]
        public float floatValue;
        [ShowIf("@ColorHelper.IsColor(type)")]
        public Color colorValue;

        public override void AppendTo(Sequence sequence)
        {
            TweenCallback callback;

            var material = useSharedMaterial ?
                target.sharedMaterial :
                target.material = new Material(target.material);
            
            if (type.IsColor())
            {
                var setter = ColorHelper.GetMaterialColorSetter(type, material);
                callback = () => setter(colorValue);
            }
            else
            {
                var setter = ColorHelper.GetMaterialFloatSetter(type, material);
                callback = () => setter(floatValue);
            }

            sequence.InsertCallback(time, callback);
        }
    }
}