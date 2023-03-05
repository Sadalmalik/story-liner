using System;
using GeekyHouse.Subsystem.Localize;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class CharacterContainer : SerializedMonoBehaviour
    {
        [SerializeField] private string containerId;

        [Space] [SerializeField] private Canvas          imageCanvas;
        [SerializeField]         private Image           image;
        [SerializeField]         private RectTransform   imageContainer;
        [SerializeField]         private bool            reversed;
        [SerializeField]         private TextMeshProUGUI name;
        [SerializeField]         private GameObject      nameConteiner;

        [Space] [SerializeField] private Color   incativeCharacterColor;
        [SerializeField]         private Vector2 activeCharacterSize;
        [SerializeField]         private Vector2 inactiveCharacterSize;

        public bool   Active      { get; private set; }
        public string ContainerId => containerId;

        public void SetCharacter(string characterId, CharacterSettings character, string emotionColor = null)
        {
            name.text = Localizer.Localize(characterId);

            Sprite sprite  = null;
            bool   reverse = false;

            if (character != null)
            {
                sprite  = character.idleEmotion;
                reverse = character.reverseHorizontal;
            }
            
            if (!string.IsNullOrEmpty(emotionColor) &&
                character!=null &&
                character.emotions.TryGetValue(emotionColor, out CharacterEmotionSettings emotionSettings))
            {
                sprite  = emotionSettings.sprite;
                reverse = emotionSettings.reverseHorizontal;
            }

            image.sprite = sprite;
            image.rectTransform.sizeDelta = sprite.rect.size;

            if (reversed)
                reverse = !reverse;

            if (reverse)
                imageContainer.localScale = new Vector3(-1, 1, 1);
            else
                imageContainer.localScale = Vector3.one;
        }

        public void SetActive(bool active)
        {
            Active = active;

            gameObject.SetActive(active);
        }

        public void SetSelected(bool active)
        {
            if (active && !Active)
                SetActive(true);

            nameConteiner.SetActive(active);
            image.color              = active ? Color.white : incativeCharacterColor;
            imageContainer.sizeDelta = active ? activeCharacterSize : inactiveCharacterSize;
            
            var pos = imageContainer.position;
            pos.z                   = active ? 1 : 0;
            imageContainer.position = pos;
        }
    }
}