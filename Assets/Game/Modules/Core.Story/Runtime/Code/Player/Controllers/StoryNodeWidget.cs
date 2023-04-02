using System;
using UnityEngine;

namespace Self.Story
{
	public abstract class StoryNodeWidget : MonoBehaviour
	{
		public event Action<string> OnNext;

		protected void InvokeNext(string nodeId)
		{
			OnNext?.Invoke(nodeId);
		}

		public abstract void SetNode(BaseNode node);
	}

	public class ReplicaWidget : StoryNodeWidget
	{
		public override void SetNode(BaseNode node)
		{
			var replica = node as ReplicaNode;
		}
	}

	public class ChoiceWidget : StoryNodeWidget
	{
		public override void SetNode(BaseNode node)
		{
			var choice = node as ChoiceNode;
		}
	}
}