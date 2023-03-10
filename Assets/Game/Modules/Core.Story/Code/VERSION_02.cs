namespace Self.Story.V2
{
	public class Node : ScriptableObject
	{
		public string id;
		public string nextNode;
		
		public List<NodeBehaviour> behaviours;
	}

	public class ReplicaNode : Node
	{
		public string character;
		public string emotion;
		public string localized;
	}

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