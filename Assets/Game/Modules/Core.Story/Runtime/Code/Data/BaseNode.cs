using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Self.StoryV2
{
    public class BaseNode : ScriptableObject
    {
        public string id;
        public List<string> nextNodes;
        public Vector2 position;
    }
}