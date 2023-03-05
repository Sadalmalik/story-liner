using System;
using System.Collections.Generic;
using GeekyHouse.Subsystem.Localize;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class QuestionNodeView : SerializedMonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private Transform _choicesContainer;
        [Space]
        [SerializeField] private DialogueChoiceUI _choicePrefab;
        [SerializeField] private CharacterContainer _questionCharacter;
        [SerializeField] private TextMeshProUGUI  questionText;
        
        private DialogueCharactersSettings       _charactersSettings;
        
        public event Action<DialogueNodeTalkChoice> onChoiceClick;
        
        private List<DialogueChoiceUI> _choices = new();

        public void SetActive(bool active)
        {
            _container.SetActive(active);
        }
        
        public void SetCharacters(DialogueCharactersSettings charactersSettings, IReadOnlyList<DialogueCharacter> characters)
        {
            _charactersSettings = charactersSettings;
        }
        
        public void SetNode(DialogueNodeMonologueWithChoices node)
        {
            questionText.text = Localizer.Localize(node.text);
            
            _questionCharacter.SetCharacter(
                node.activeCharacter,
                _charactersSettings.characters[node.activeCharacter],
                node.emotionColor);
            _questionCharacter.SetSelected(true);
            
            int counter = 0;
            for (; counter < node.choices.Count; counter++)
            {
                DialogueNodeTalkChoice choice = node.nodeChoices[counter];

                if (_choices.Count == counter)
                {
                    _choices.Add(Instantiate(_choicePrefab, _choicesContainer));
                    _choices[counter].onClick += HandleChoiceClick;
                }

                DialogueChoiceUI choiceUI = _choices[counter];
                choiceUI.gameObject.SetActive(true);
                choiceUI.Set(choice);
            }

            for (int i = counter; i < _choices.Count; i++)
            {
                _choices[counter].gameObject.SetActive(false);
            }
        }
        
        private void HandleChoiceClick(DialogueNodeTalkChoice choice)
        {
            onChoiceClick?.Invoke(choice);
        }
    }
}