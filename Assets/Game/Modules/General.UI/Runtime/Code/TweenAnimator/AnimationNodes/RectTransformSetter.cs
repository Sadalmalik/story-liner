// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
    public class RectTransformSetter : AnimationNode
    {
		[Space]
		public RectTransform target;
		public bool local;
		public RectTransformDataType type = RectTransformDataType.Position;
		
		[ShowIf("@DataTypesExtensions.IsVector1(type)")]
		public float floatValue;
        [ShowIf("@DataTypesExtensions.IsVector2(type)")]
		public Vector2 vector2Value;
        [ShowIf("@DataTypesExtensions.IsVector3(type)")]
		public Vector3 vector3Value;

		public override void AppendTo(Sequence sequence)
		{
			TweenCallback callback;

			if (type.IsVector3())
			{
				var setter = TransformHelper.GetTransformVectorSetter((TransformDataType) type, target, local);
				callback = () => setter(vector3Value);
			}
			else
			{
				var setter = TransformHelper.GetRectTransformFloatSetter(type, target, local);
				callback = () => setter(floatValue);
			}

			sequence.InsertCallback(time, callback);
		}
    }
}