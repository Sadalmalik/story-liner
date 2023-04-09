using System;
using UnityEngine;

namespace Self.Story
{
	public abstract class StoryNodeWidget : MonoBehaviour
	{
		public Type TargetType => typeof(ReplicaNode);
		
		public event Action<string> OnNext;

		protected void InvokeNext(string nodeId)
		{
			OnNext?.Invoke(nodeId);
		}

		public abstract void SetNode(BaseNode node);
	}
}