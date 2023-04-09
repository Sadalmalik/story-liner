// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Kaleb.TweenAnimator
{
	public class TransformSetter : AnimationNode
	{
		[Space]
		public Transform target;
		public TransformDataType type = TransformDataType.Position;
		public bool local;

		[Space]
        [ShowIf("@DataTypesExtensions.IsVector1(type)")]
		public float floatValue;
        [ShowIf("@DataTypesExtensions.IsVector3(type)")]
		public Vector3 vectorValue;

		public override void AppendTo(Sequence sequence)
		{
			TweenCallback callback;

			if (type.IsVector3())
			{
				var setter = TransformHelper.GetTransformVectorSetter(type, target, local);
				callback = () => setter(vectorValue);
			}
			else
			{
				var setter = TransformHelper.GetTransformFloatSetter(type, target, local);
				callback = () => setter(floatValue);
			}

			sequence.InsertCallback(time, callback);
		}
	}
}