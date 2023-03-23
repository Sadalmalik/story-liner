using Self.Story;
using UnityEngine;

namespace Self.StoryV2
{
    [System.Serializable]
    public class ChopChop
    {
        public string chop1;
        public string chop2;
    }

    public class SetVariableAction : NodeAction
    {
        [DisplayOnNode(1)] public string variableName;
        [DisplayOnNode(2)] public string variableValue;

        public override void OnExecute()
        {
            Debug.Log("Chop Chop!");
        }
    }
}