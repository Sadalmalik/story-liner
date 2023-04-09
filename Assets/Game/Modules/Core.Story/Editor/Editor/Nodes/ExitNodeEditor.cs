using UnityEditor;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    [InspectedType(typeof(ExitNode))]
    [CustomEditor(typeof(ExitNode))]
    public class ExitNodeEditor : NodeEditor
    {
        protected override void CreateNodeGUI(VisualElement root)
        {
            // do not display anything
        }
    }
}