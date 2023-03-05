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
    public class DialogueUI : SerializedMonoBehaviour
    {
        [SerializeField] private MonologueNodeView _monologueNodeView;
        [SerializeField] private QuestionNodeView  _questionNodeView;
        
        [SerializeField] private Button skipButton;

        private DialogueCharactersSettings _charactersSettings;

        private IReadOnlyList<DialogueCharacter> _characters;

        public event Action                         onNextClick;
        public event Action<DialogueNodeTalkChoice> onChoiceClick;
        public event Action                         onSkipClick;

        private void Awake()
        {
            Hide();
            
            skipButton.onClick.AddListener(OnSkipClick);
            
            _charactersSettings = Resources.Load<DialogueCharactersSettings>("DialogueCharactersSettings");

            _monologueNodeView.onNextClick  += HandleNextClick;
            _questionNodeView.onChoiceClick += HandleChoiceClick;
        }

        public void Hide()
        {
            skipButton.gameObject.SetActive(false);
            _monologueNodeView.SetActive(false);
            _questionNodeView.SetActive(false);
        }
        
        public void SetCharacters(IReadOnlyList<DialogueCharacter> characters)
        {
            _characters = characters;
            
            _monologueNodeView.SetCharacters(_charactersSettings, characters);
            _questionNodeView.SetCharacters(_charactersSettings, characters);
        }

        public void SetNode(DialogueNode node)
        {
            skipButton.gameObject.SetActive(true);
            
            switch (node)
            {
                case DialogueNodeMonologueWithChoices monologueWithChoices:
                    _monologueNodeView.SetActive(false);
                    _questionNodeView.SetActive(true);
                    _questionNodeView.SetNode(monologueWithChoices);
                    break;
                case DialogueNodeMonologue monologue:
                    _monologueNodeView.SetActive(true);
                    _questionNodeView.SetActive(false);
                    _monologueNodeView.SetNode(monologue);
                    break;
                default:
                    Debug.LogError($"Dialogues: Unexpected node for UI - {node.GetType()}");
                    break;
            }
        }

        private void HandleChoiceClick(DialogueNodeTalkChoice choice)
        {
            onChoiceClick?.Invoke(choice);
        }

        private void OnSkipClick()
        {
            onSkipClick?.Invoke();
        }

        private void HandleNextClick()
        {
            onNextClick?.Invoke();
        }
    }
}