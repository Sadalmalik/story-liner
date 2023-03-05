using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    [CreateAssetMenu(fileName = "DialogueCharactersSettings", menuName = "[GeekyHouse]/Dialogue/CharactersSettings", order = 1)]
    public class DialogueCharactersSettings : SerializedScriptableObject
    {
        public Dictionary<string, CharacterSettings> characters = new Dictionary<string, CharacterSettings>();
    }

    [Serializable]
    public class CharacterSettings
    {
        public Sprite idleEmotion;
        public bool   reverseHorizontal;

        public Dictionary<string, CharacterEmotionSettings> emotions =
            new Dictionary<string, CharacterEmotionSettings>();
    }

    [Serializable]
    public class CharacterEmotionSettings
    {
        public Sprite sprite;
        public bool   reverseHorizontal;
    }
}