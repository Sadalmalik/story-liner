using TMPro;
using UnityEngine;

namespace Self.Story
{
	public class StoryView : MonoBehaviour
	{
		[Header("General widgets")]
		public BackgroundWidget backgroundWidget;

		[Header("Story widgets")]
		public PauseWidget   pauseWidget;
		public ReplicaWidget replicaWidget;
		public ChoiceWidget  choiceWidget;

		[Header("Story widgets")]
		public TMP_Text endMessage;
	}
}