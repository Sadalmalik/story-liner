using UnityEngine.UIElements;

namespace Self.Story.Editors
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }



        public SplitView()
        {

        }
    }
}