using UnityEditor;
using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    [InspectedType(typeof(SubChapterNode))]
    [CustomEditor(typeof(SubChapterNode))]
    public class SubChapterNodeEditor : NodeEditor
    {
        protected override void CreateNodeGUI(VisualElement root)
        {
            // TODO: [Andrei]
            // maybe display some info about containing nodes here?
        }
    }
}