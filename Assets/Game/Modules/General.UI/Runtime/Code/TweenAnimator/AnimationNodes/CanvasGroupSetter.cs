// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class CanvasGroupSetter : AnimationNode
    {
        [Space]
        public CanvasGroup target;

        [Space]
        public float floatValue;

        public override void AppendTo(Sequence sequence)
        {
            TweenCallback callback;

            callback = () => target.alpha = floatValue;

            sequence.InsertCallback(time, callback);
        }
    }
}