using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	public class Node : ScriptableObject
	{
		public string              id;
		[HideInInspector]
		public List<string>        nextNodes;
		public NodeBehaviour       mainBehaviour;
		public List<NodeBehaviour> behaviours;

		#region EDITOR_PARAMETERS

		[HideInInspector] 
		public Vector2 position;

		#endregion
	}
}