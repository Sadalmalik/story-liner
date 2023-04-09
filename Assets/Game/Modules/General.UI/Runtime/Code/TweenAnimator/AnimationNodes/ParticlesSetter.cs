// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using DG.Tweening;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
	public class ParticlesSetter : AnimationNode
	{
		[Space]
		public ParticleSystem particles;

		public bool play = true;

		public override void AppendTo(Sequence sequence)
		{
			bool startPlay = play;
			sequence.InsertCallback(time, () =>
			{
				if (startPlay)
                {
                    particles.Clear(true);
					particles.Play(true);
                }
				else
                {
					particles.Stop(true);
                    particles.Clear(true);
                }
			});
		}
	}
}