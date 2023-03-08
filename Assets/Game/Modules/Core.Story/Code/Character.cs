using UnityEngine;

namespace Self.Story
{
    [CreateAssetMenu(fileName = "Character_", menuName = "Story/Configs/Character/New")]
    public class Character : ScriptableObject
    {
        public string characterName;
        public string defaultSkin;
        public string defaultEmotion;
    }
}