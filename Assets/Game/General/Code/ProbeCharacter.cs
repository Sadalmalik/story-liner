using System;
using System.Linq;
using Self.Story;
using UnityEngine;

namespace Self.Game
{
	public class ProbeCharacter : MonoBehaviour
	{
		public Character       character;
		public CharacterWidget widget;
		public int             currentEmotion;
		public bool            toggleCharacter;
		public bool            toggleCharacterDirection;
		public bool            toggleCharacterEmotion;

		
		public void Update()
		{
			Trigger(ref toggleCharacter, ToggleCharacter);
			Trigger(ref toggleCharacterDirection, ToggleCharacterDirection);
			Trigger(ref toggleCharacterEmotion, ToggleCharacterEmotion);
		}

		private void ToggleCharacter()
		{
			currentEmotion = 0;
			widget.SetCharacter(character);
		}
		
		private void ToggleCharacterDirection()
		{
			widget.SetDirection(widget.direction.Mirror());
		}
		
		private void ToggleCharacterEmotion()
		{
			// гавнокоооод... гавноокоооод...
			// Единственный в мире малыш гавнокод!
			var emotion = widget.character.emotions.FirstOrDefault(e => e.sprite == widget.view.sprite);
			currentEmotion = widget.character.emotions.ToList().IndexOf(emotion);
			currentEmotion = (currentEmotion + 1) % widget.character.emotions.Length;
			widget.SetEmotion(widget.character.emotions[currentEmotion].name);
		}

		private void Trigger(ref bool trigger, Action act)
		{
			if (trigger) { trigger = false; act();}
		}
	}
}