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

		public string  chapterName;
		public string  startNodeID;
		public Chapter parentChapter;

		public List<BaseNode> nodes = new();

		[NonSerialized]
		private Dictionary<string, BaseNode> _nodesByID = null;

		public Dictionary<string, BaseNode> NodesByID
		{
			get
			{
				if (_nodesByID == null)
				{
					_nodesByID = new();
					foreach (var node in nodes)
						if (node != null && node.id != null)
							NodesByID?.Add(node.id, node);
				}

				return _nodesByID;
			}
		}

		public void AddNode(BaseNode node)
		{
			NodesByID.Add(node.id, node);
			nodes.Add(node);
		}

		public void RemoveNode(string nodeId)
		{
			nodes.Remove(NodesByID[nodeId]);
			NodesByID.Remove(nodeId);
		}

		public void RemoveNode(BaseNode node)
		{
			nodes.Remove(node);
			NodesByID.Remove(node.id);
		}

		public BaseNode TryGetNode(string nodeID)
		{
			if (NodesByID.TryGetValue(nodeID, out var node))
				return node;
			if (parentChapter != null)
				return parentChapter.TryGetNode(nodeID);
			return null;
		}
	}
}