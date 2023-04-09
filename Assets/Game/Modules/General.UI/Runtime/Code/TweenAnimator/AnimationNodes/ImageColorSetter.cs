// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Kaleb.TweenAnimator
{
	public class ImageColorSetter : AnimationNode
	{
		[Space]
		public Image target;
		public ColorDataType type = ColorDataType.Color;

		[Space]
        [ShowIf("@!ColorHelper.IsColor(type)")]
		public float floatValue;
        [ShowIf("@ColorHelper.IsColor(type)")]
		public Color colorValue;

		public override void AppendTo(Sequence sequence)
		{
			TweenCallback callback;

			if (type.IsColor())
			{
				var setter = ColorHelper.GetImageColorSetter(type, target);
				callback = () => setter(colorValue);
			}
			else
			{
				var setter = ColorHelper.GetImageFloatSetter(type, target);
				callback = () => setter(floatValue);
			}

			sequence.InsertCallback(time, callback);
		}
	}
}