using System;
using System.Collections.Generic;
using GeekyHouse.Subsystem.Save;
using Sirenix.OdinInspector;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class DialogueConfig : SerializedScriptableObject
    {
        public string                        id;
        public string                        firstNode;
        public List<string>                  lastNodes  = new List<string>();
        public List<DialogueCharacter>       characters = new List<DialogueCharacter>();
        public Dictionary<int, DialogueNode> nodes      = new Dictionary<int, DialogueNode>();
    }

    public abstract class DialogueNode : SaveObject
    {
        public string id;
    }

    public abstract class DialogueNodeWithNext : DialogueNode
    {
        public string nextNode;
    }

    public abstract class DialogueNodeTalk : DialogueNodeWithNext
    {
        public string text;
    }

    public class DialogueNodeInstruction : DialogueNodeWithNext
    {
        public List<DialogueValue>      localValues = new List<DialogueValue>();
        public List<DialogueGlobalFlag> globalFlags = new List<DialogueGlobalFlag>();
    }

    public class DialogueNodeCondition : DialogueNode
    {
        public string nodeTrue;
        public string nodeFalse;
    }

    public class DialogueNodeConditionLocal : DialogueNodeCondition
    {
        public DialogueValue value;
    }

    public class DialogueNodeConditionGlobal : DialogueNodeCondition
    {
        public DialogueGlobalFlag value;
    }

    public class DialogueNodeMonologue : DialogueNodeTalk
    {
        public string activeCharacter;
        public string emotionColor;
        public string text;
    }

    public class DialogueNodeMonologueWithChoices : DialogueNodeMonologue
    {
        public                 List<string>                 choices     = new List<string>();
        [NonSerialized] public List<DialogueNodeTalkChoice> nodeChoices = new List<DialogueNodeTalkChoice>();
    }

    public class DialogueNodeTalkChoice : DialogueNodeTalk {}

    public class DialogueCharacter
    {
        public DialogueCharacter(string name, string position)
        {
            this.name     = name;
            this.position = position;
        }
    
        public string name;
        public string position;
    }

    public class DialogueValue
    {
        public DialogueValue(string type, string name, string value)
        {
            this.type  = type;
            this.name  = name;
            this.value = value;
        }
    
        public string type;
        public string name;
        public string value;
        
        public static bool operator ==(DialogueValue value1, DialogueValue value2)
        {
            return value1.name == value2.name && value1.type == value2.type && value1.value == value2.value;
        }

        public static bool operator !=(DialogueValue value1, DialogueValue value2)
        {
            return !(value1 == value2);
        }
    }

    public class DialogueGlobalFlag
    {
        public string name;
        public bool   value;
    }
}