using System.Collections.Generic;

namespace Self.StoryV2
{
	public class ChoiceNode : ReplicaNode
	{
		public List<Choice> choices;
		
		public class Choice
		{
			public string localizedText;
			public string nextNode;
		}
	}
}