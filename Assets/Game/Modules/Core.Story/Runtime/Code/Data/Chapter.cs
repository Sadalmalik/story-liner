using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu(
		menuName = "[SELF]/Story/New Chapter",
		fileName = "New Chapter B",
		order = 2)]
	public class Chapter : ScriptableObject
	{
		public Book book;

		public string chapterName;
		public string startNodeID;

		public List<BaseNode> nodes = new();

		public Chapter parentChapter;

		[NonSerialized] public Dictionary<string, BaseNode> nodesByID = new();

		public void AddNode(BaseNode node)
		{
			nodes.Add(node);
			nodesByID.Add(node.id, node);
		}

		public void RemoveNode(string nodeId)
		{
			nodes.Remove(nodesByID[nodeId]);
			nodesByID.Remove(nodeId);
		}

		public void RemoveNode(BaseNode node)
		{
			nodes.Remove(node);
			nodesByID.Remove(node.id);
		}

		public void Init()
		{
			nodesByID.Clear(); 
			foreach (var node in nodes)
				if (node != null && node.id != null)
					nodesByID?.Add(node.id, node);
		}

		public BaseNode TryGetNode(string nodeID)
		{
			if (nodesByID.TryGetValue(nodeID, out var node))
				return node;
			if (parentChapter != null)
				return parentChapter.TryGetNode(nodeID);
			return null;
		}
	}
}