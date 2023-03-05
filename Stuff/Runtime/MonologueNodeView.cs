using System;
using System.Collections.Generic;
using System.Linq;
using GeekyHouse.Subsystem.Localize;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class MonologueNodeView : SerializedMonoBehaviour
    {
        [SerializeField] private GameObject _container;
        
        [Space]
        
        [SerializeField] private CharacterContainer Left_1;
        [SerializeField] private CharacterContainer Left_2;
        [SerializeField] private CharacterContainer Right_1;
        [SerializeField] private CharacterContainer Right_2;
        [Space]
        [SerializeField] private TextMeshProUGUI _monologueText;

        [SerializeField] private Button _nextButton;

        private DialogueCharactersSettings       _charactersSettings;
        private IReadOnlyList<DialogueCharacter> _characters;

        public event Action onNextClick;

        private void Awake()
        {
            _nextButton.onClick.AddListener(OnNextButtonClick);
        }

        public void SetActive(bool active)
        {
            _container.SetActive(active);
        }
        
        public void SetCharacters(
            DialogueCharactersSettings charactersSettings,
            IReadOnlyList<DialogueCharacter>                 characters)
        {
            _charactersSettings = charactersSettings;
            _characters         = characters;

            Left_1.SetActive(false);
            Left_2.SetActive(false);
            Right_1.SetActive(false);
            Right_2.SetActive(false);
        }

        private CharacterSettings GetCharacter(string character)
        {
            if (_charactersSettings.characters.TryGetValue(character, out var settings))
                return settings;
            return null;
        }
        
        public void SetNode(DialogueNodeMonologue node)
        {
            _monologueText.text = Localizer.Localize(node.text);

            string activeCharacter = node.activeCharacter;
            string position        = _characters.First(e => e.name == activeCharacter).position;

            if (Left_1.ContainerId.Equals(position))
            {
                Left_1.SetCharacter(node.activeCharacter, GetCharacter(activeCharacter), node.emotionColor );
            }
            else if (Left_2.ContainerId.Equals(position))
            {
                Left_2.SetCharacter(node.activeCharacter, GetCharacter(activeCharacter), node.emotionColor );
            }
            else if (Right_1.ContainerId.Equals(position))
            {
                Right_1.SetCharacter(node.activeCharacter, GetCharacter(activeCharacter), node.emotionColor );
            }
            else if (Right_2.ContainerId.Equals(position))
            {
                Right_2.SetCharacter(node.activeCharacter, GetCharacter(activeCharacter), node.emotionColor );
            }

            Left_1.SetSelected(position.Equals(Left_1.ContainerId));
            Left_2.SetSelected(position.Equals(Left_2.ContainerId));
            Right_1.SetSelected(position.Equals(Right_1.ContainerId));
            Right_2.SetSelected(position.Equals(Right_2.ContainerId));
        }

        private void OnNextButtonClick()
        {
            onNextClick?.Invoke();
        }
    }
}