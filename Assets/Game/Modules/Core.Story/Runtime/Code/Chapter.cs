using Self.StoryV2;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu]
	public class Chapter : ScriptableObject
	{
		public string chapterName;
		public string startNode;

		public Dictionary<string, Node> nodesByID;

		public List<Variable> variables;
		public List<Node>     nodes;

		public void Init()
		{
			nodesByID = new Dictionary<string, Node>();
			foreach (var node in nodes)
				nodesByID.Add(node.id, node);
		}
	}
}