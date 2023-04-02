using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	public class BaseNode : ScriptableObject
	{
		public string  id;
		public Vector2 position;

		[HideInInspector]
		public List<string> nextNodes = new();
	}
}