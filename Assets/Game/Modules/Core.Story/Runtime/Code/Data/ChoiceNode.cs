using Self.Story;
using System.Collections.Generic;

namespace Self.StoryV2
{
	public class ChoiceNode : ReplicaNode
	{
		public List<Choice> choices;
		
		[System.Serializable]
		public class Choice
		{
			public string localizedText;

			// TODO [Andrei]: should just take nextNode
			// from index
			//public string nextNode;
		}
	}
}