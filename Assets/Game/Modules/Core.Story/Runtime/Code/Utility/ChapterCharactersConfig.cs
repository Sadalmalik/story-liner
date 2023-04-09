using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu(
		menuName = "[SELF]/Story/New Characters list",
		fileName = "New Characters list",
		order = 2)]
	public class ChapterCharactersConfig : ScriptableObject
	{
		public List<Character> characters;
	}
}