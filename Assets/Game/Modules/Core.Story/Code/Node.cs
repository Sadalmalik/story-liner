using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	public class Node
	{
		public string              id;
		public List<string>        nextNodes;
		public List<NodeBehaviour> behaviours;
	}
}