using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu(
		menuName = "[SELF]/Story/New Book",
		fileName = "New Book",
		order = 2)]
	public class Book : ScriptableObject
	{
		public string bookName;

		public List<Character> characters;

		public VariablesContainer variables;
	}
}