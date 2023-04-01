using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu]
	public class Book : ScriptableObject
	{
		public string bookName;

		public List<Character> characters;

		public VariablesContainer variables;
	}
}