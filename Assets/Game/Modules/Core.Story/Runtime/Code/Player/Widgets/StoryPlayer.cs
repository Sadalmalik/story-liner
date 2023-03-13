using System;
using Self.Architecture.IOC;
using UnityEngine;

namespace Self.StoryV2
{
	public class StoryPlayer : MonoBehaviour
	{
		private Chapter     _chapter;
		private ChapterSave _save;
		private string      _currentNode;

		private StoryNodeWidget _activeWidget;

		public ReplicaWidget replicaWidget;
		public ChoiceWidget  choiceWidget;

		public event Action<string> OnStoryBroken;

		public void SetChapter(Chapter chapter, ChapterSave save)
		{
			_chapter     = chapter;
			_currentNode = save.currentNode ?? chapter.startNode;

			SetNode(_currentNode);
		}

		public void SetNode(string nodeId)
		{
			if (!_chapter.nodesByID.TryGetValue(nodeId, out var node))
			{
				Debug.Log("Story was broken!");
				OnStoryBroken?.Invoke(_currentNode);
				return;
			}

			if (node is Node)
			{
				
			}
			else if (node is ReplicaNode replica)
			{
				
			}
			else if (node is ChoiceNode choice)
			{
				
			}
			
			if (node.GetType() == typeof(Node))
			{
				Debug.Log("Node");
			}
			else if (node.GetType() == typeof(ReplicaNode))
			{
				Debug.Log("Node");
			}
			else if (node.GetType() == typeof(ChoiceNode))
			{
				Debug.Log("Node");
			}
		}
	}
}