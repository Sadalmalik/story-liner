using System;
using System.Linq;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu(
		menuName = "[SELF]/Story/New Character",
		fileName = "New Character",
		order = 1)]
	public class Character : ScriptableObject
	{
		public string characterName;
		public Sprite characterIcon;
		public bool   isMainCharacter;

		public Emotion[] emotions;

		public Emotion GetEmotion(string emotion)
		{
			return emotions.FirstOrDefault(e => e.name.Equals(emotion));
		}
	}

	[Serializable]
	public class Emotion
	{
		public string name;
		public Sprite sprite;
	}

	[System.Serializable]
	public class EmotionReference
	{
		public string emotion;
	}

	[System.Serializable]
	public class CharacterReference
	{
		public Character character;
	}
}