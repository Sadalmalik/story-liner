using Self.Story;
using UnityEngine;

namespace Self.StoryV2
{
    [System.Serializable]
    public class VariableReference
    {
        public Variable variable;
    }

    public class SetVariableAction : NodeAction
    {
        [DisplayOnNode(1), SerializeReference] public VariableReference variableReference;
        [DisplayOnNode(2)] public string variableValue;

        public override void OnExecute()
        {

        }
    }
}