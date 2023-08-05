using Self.Architecture.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
    public class CharacterWidget : AnimatedWidget
    {
        public RectTransform LeftContainer;
        public RectTransform RightContainer;
        public RectTransform CharacterRoot;
        public Image CharacterImage;



        public void InitCharacter(Character character)
        {
			// TODO: [Andrei]
			// detect if we need to change the character/emotion
			// and do so 
            CharacterImage.sprite = character.characterIcon;

            CharacterRoot.gameObject.SetActive(CharacterImage.sprite != null);
            CharacterRoot.FitInside(character.isMainCharacter ? LeftContainer : RightContainer);
        }
    }
}
