using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class CharacterWidget : MonoBehaviour
	{
		public Image view;

		public Character character { get; private set; }
		public Direction direction { get; private set; }

		public void SetCharacter(Character character)
		{
			this.character = character;

			SetEmotion();
			SetDirection(Direction.Right);
		}

		public void SetEmotion(string emotionId = null)
		{
			var emotion = character.GetEmotion(emotionId);
			if (emotion != null)
				view.sprite = emotion.sprite;
			else
				view.sprite = character.characterIcon;
		}

		public void SetDirection(Direction direction = Direction.Right)
		{
			this.direction = direction;
			switch (direction)
			{
				case Direction.Left:
					view.rectTransform.localScale = new Vector3(-1, 1, 1);
					break;
				case Direction.Right:
				case Direction.None:
					view.rectTransform.localScale = Vector3.one;
					break;
			}
		}

		public enum Direction
		{
			None  = 0,
			Left  = 1,
			Right = 2
		}
	}

	public static class CharacterDirectionExtension
	{
		public static CharacterWidget.Direction Mirror(this CharacterWidget.Direction direction)
		{
			switch (direction)
			{
				case CharacterWidget.Direction.Left:
					return CharacterWidget.Direction.Right;
				case CharacterWidget.Direction.Right:
					return CharacterWidget.Direction.Left;
			}
			return CharacterWidget.Direction.None;
		}
	}
}