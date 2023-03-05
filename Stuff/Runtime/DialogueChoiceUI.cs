using System;
using GeekyHouse.Subsystem.Localize;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class DialogueChoiceUI : SerializedMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button          button;

        public event Action<DialogueNodeTalkChoice> onClick;

        private DialogueNodeTalkChoice _nodeChoice;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        public void Set(DialogueNodeTalkChoice nodeChoice)
        {
            text.text = Localizer.Localize(nodeChoice.text);

            _nodeChoice = nodeChoice;
        }

        private void OnButtonClick()
        {
            onClick?.Invoke(_nodeChoice);
        }
    }
}