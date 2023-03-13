using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.StoryV2
{
	[CreateAssetMenu]
	public class Chapter : ScriptableObject
	{
		public string chapterName;
		public string startNode;

		public List<Node> nodes;
		//public List<ChapterSettings> settings;
		public List<Variable> variables;
        
		[NonSerialized]
		public Dictionary<string, Node> nodesByID;

		public void Init()
		{
			nodesByID = new Dictionary<string, Node>();
			foreach (var node in nodes)
				nodesByID.Add(node.id, node);
		}
	}
}