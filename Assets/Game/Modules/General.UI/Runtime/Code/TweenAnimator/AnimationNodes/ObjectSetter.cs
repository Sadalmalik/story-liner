// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class ObjectSetter : AnimationNode
    {
        [Space]
        public GameObject target;

        public bool setActive = true;

        public override void AppendTo(Sequence sequence)
        {
            sequence.InsertCallback(time, () => { target.SetActive(setActive); });
        }
    }
}