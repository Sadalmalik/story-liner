using UnityEngine;

namespace Self.StoryV2
{
    public class SetVariableAction : NodeAction
    {
        public string variableName;
        public string variableValue;

        public override void OnExecute()
        {
            Debug.Log("Chop Chop!");
        }
    }
}