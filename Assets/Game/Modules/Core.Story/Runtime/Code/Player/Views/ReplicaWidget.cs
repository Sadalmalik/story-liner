using Self.Architecture.Utils;
using TMPro;

namespace Self.Story
{
	public class ReplicaWidget : StoryNodeWidget
	{
		public TMP_Text text;

		public float duration;
		
		public override void SetNode(BaseNode node)
		{
			var replica = node as ReplicaNode;

			text.DOText(replica.localized, duration);
		}
	}
}