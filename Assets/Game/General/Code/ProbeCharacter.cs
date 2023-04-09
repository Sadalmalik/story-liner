using System;
using System.Linq;
using Self.Architecture.Utils;
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
			toggleCharacter.Trigger(ToggleCharacter);
			toggleCharacterDirection.Trigger(ToggleCharacterDirection);
			toggleCharacterEmotion.Trigger(ToggleCharacterEmotion);
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
	}
}