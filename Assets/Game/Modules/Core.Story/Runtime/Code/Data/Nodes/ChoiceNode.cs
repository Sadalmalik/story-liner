using Self.Story;
using System.Collections.Generic;

namespace Self.Story
{
	/// <summary>
	/// A node that contains character dialogue
	/// as well as some player choices
	/// </summary>
	[NodeMetadata(customOutput: true)]
	public class ChoiceNode : ReplicaNode
	{
		public List<Choice> choices;

		[System.Serializable]
		public class Choice
		{
			public string localizedText;
		}
	}
}
