using UnityEngine;

namespace Self.Story
{
	public class StoryView : MonoBehaviour
	{
		[Header("General widgets")]
		public BackgroundWidget backgroundWidget;
		
		[Header("Story widgets")]
		public ReplicaWidget replicaWidget;
		public ChoiceWidget choiceWidget;
	}
}