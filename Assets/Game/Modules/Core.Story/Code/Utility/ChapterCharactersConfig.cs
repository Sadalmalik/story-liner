using System.Collections.Generic;
using UnityEngine;

namespace Self.Story.Utility
{
    [CreateAssetMenu(fileName = "CharConfig_", menuName = "Story/Configs/Chapter/Character Config")]
    public class ChapterCharactersConfig : ScriptableObject
    {
        public List<Character> characters;
    }
}