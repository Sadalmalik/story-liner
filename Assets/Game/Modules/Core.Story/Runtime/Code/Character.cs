using UnityEngine;

namespace Self.Story
{
    [System.Serializable]
    public class EmotionReference
    {
        public string emotion;
    }

    [CreateAssetMenu(fileName = "Character_", menuName = "Story/Configs/Character/New")]
    public class Character : ScriptableObject
    {
        public string characterName;
        public string defaultSkin;
        public EmotionReference defaultEmotion;
        public Sprite characterIcon;
        public Sprite characterSprite;

        public string[] emotions;
    }
}