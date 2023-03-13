using System;
using UnityEngine;

namespace Self.StoryV2
{
	public abstract class StoryNodeWidget : MonoBehaviour
	{
		public event Action<string> OnNext;

		protected void InvokeNext(string nodeId)
		{
			OnNext?.Invoke(nodeId);
		}

		public abstract void SetNode(Node node);
	}

	public class ReplicaWidget : StoryNodeWidget
	{
		public override void SetNode(Node node)
		{
			var replica = node as ReplicaNode;
		}
	}
	
	public class ChoiceWidget : StoryNodeWidget
	{
		public override void SetNode(Node node)
		{
			var choice = node as ChoiceNode;
		}
	}
}