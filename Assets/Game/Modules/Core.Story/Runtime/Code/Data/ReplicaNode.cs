using Self.Story;
using UnityEngine;

namespace Self.StoryV2
{
    [NodeMetadata]
	public class ReplicaNode : Node
	{
		[SerializeReference] public CharacterReference character;
		[SerializeReference] public EmotionReference emotion;
		public string localized;
	}
}