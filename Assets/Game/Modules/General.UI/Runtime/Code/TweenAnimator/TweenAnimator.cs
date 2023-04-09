// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kaleb.TweenAnimator
{
	public class TweenAnimator : SerializedMonoBehaviour
	{
		public int  loops       = 1;
		public bool playOnAwake = false;

		[PropertySpace(SpaceBefore = 10, SpaceAfter = 0),
		 HorizontalGroup(group: "Components", order: 1)]
		public List<AnimationNode> nodes = new List<AnimationNode>();

		public Sequence sequence { get; private set; }

		public float Duration => sequence.Duration();

		public event Action OnComplete;

		private void Awake()
		{
			RebuildSequence();
			
			if (playOnAwake)
				Play();
		}

		private void InvokeComplete()
		{
			OnComplete?.Invoke();
			
		}

		[EnableIf("@UnityEngine.Application.isPlaying"),
		 HorizontalGroup(group: "Buttons", order: 0),
		 Button]
		public void RebuildSequence()
		{
			if (sequence != null)
			{
				sequence.onComplete = null;
				sequence.Kill();
			}

			sequence = DOTween.Sequence();
			sequence.SetAutoKill(false);
			sequence.SetRecyclable(true);
			foreach (var node in nodes)
				node.AppendTo(sequence);
			sequence.SetLoops(loops);
			sequence.onComplete = InvokeComplete;
			sequence.Pause();
		}

		[EnableIf("@UnityEngine.Application.isPlaying"),
		 HorizontalGroup(group: "Buttons", order: 0),
		 Button]
		public void RebuildAndPlay()
		{
			RebuildSequence();
			Play();
		}

		[EnableIf("@UnityEngine.Application.isPlaying"),
		 HorizontalGroup(group: "Buttons", order: 0),
		 Button]
		public void Play()
		{
			if (sequence == null)
				RebuildSequence();
			sequence.Restart();
			sequence.Play();
		}

		public void Stop()
		{
			if (sequence == null)
				RebuildSequence();
			sequence.Restart();
			sequence.Pause();
        }

        public bool IsPlaying => sequence != null && sequence.IsPlaying();
    }
}