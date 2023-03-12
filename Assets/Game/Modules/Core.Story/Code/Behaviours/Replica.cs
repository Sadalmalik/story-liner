namespace Self.Story
{
	public class Replica : NodeBehaviour
	{
        [DisplayOnNode] public CharacterReference characterId;
		[DisplayOnNode] public string characterEmotion;
		[DisplayOnNode] public string localizationTerm;

		public override void OnFirstEnter()
		{
		}

		public override void OnEnter()
		{
		}

		public override void OnExit()
		{
		}
	}
}