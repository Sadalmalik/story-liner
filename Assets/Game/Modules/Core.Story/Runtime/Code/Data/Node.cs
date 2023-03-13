using System.Collections.Generic;
using UnityEngine;

namespace Self.StoryV2
{
	public class Node : ScriptableObject
	{
		public string id;
		public string nextNode;
		
		public List<NodeAction> behaviours;
	}
}