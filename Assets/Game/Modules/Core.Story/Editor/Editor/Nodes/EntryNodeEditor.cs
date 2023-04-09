using UnityEditor;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    [InspectedType(typeof(EntryNode))]
    [CustomEditor(typeof(EntryNode))]
    public class EntryNodeEditor : NodeEditor
    {
        protected override void CreateNodeGUI(VisualElement root)
        {
            // do not display anything
        }
    }
}