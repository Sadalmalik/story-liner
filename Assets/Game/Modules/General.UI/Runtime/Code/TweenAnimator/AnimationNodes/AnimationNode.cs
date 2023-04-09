// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;

namespace Kaleb.TweenAnimator
{
	public abstract class AnimationNode
	{
		public float time;

		public abstract void AppendTo(Sequence sequence);
	}
}