using System.Linq;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class PlayAnimationAction : BaseAction
	{
		[DisplayOnNode(1)] public string AnimationName;

		[Inject] private StoryController _storyController;



		public override void Execute(BaseNode node)
		{
			var anim = _storyController.StoryView.animations.FirstOrDefault(a => a.name == AnimationName);

			anim?.Play();
		}

		public override object Clone()
		{
			var clone = (PlayAnimationAction)base.Clone();

			clone.AnimationName = (string)this.AnimationName.Clone();

			return clone;
		}
	}
}