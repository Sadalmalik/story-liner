using Self.Story;
using UnityEngine;

namespace Self.Story
{
	[NodeMetadata]
	public class ReplicaNode : Node
	{
		[SerializeReference] public CharacterReference character;
		[SerializeReference] public EmotionReference   emotion;

		public string localized;
	}
}