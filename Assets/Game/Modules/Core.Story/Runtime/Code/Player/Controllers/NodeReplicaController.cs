using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ReplicaNode);

		[Inject] private StoryController _StoryController;

		public ReplicaNode   Node { get; private set; }
		public ReplicaWidget View { get; private set; }

        protected bool _isPlayingSameNode;



		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			View = signal.view.replicaWidget;

			View.OnClick        += HandleClick;
			View.OnShowComplete += HandleShowComplete;
			View.OnHideComplete += HandleHideComplete;
		}

		public string Enter(BaseNode node)
		{
			Node = node as ReplicaNode;
			View.Show(Node, _isPlayingSameNode);
            _isPlayingSameNode = false;

            return null;
		}
		
		private void HandleClick()
		{
            var nextNode = _StoryController.CurrentChapter.TryGetNode(Node.NextNode);
            var nodeType = nextNode.GetType();

            if(nodeType == GetTargetType())
            {
                // same node playing
                // check if we need to change anything
                // or just clear the text 
                var replicaNode = nextNode as ReplicaNode;

                var needCharacterChange = replicaNode.character.characterName != Node.character.characterName;
                var needEmotionChange = replicaNode.emotion?.emotion != Node.emotion?.emotion;

                if(needCharacterChange || needEmotionChange)
                {
                    View.Hide();
                }
                else
                {
                    _isPlayingSameNode = true;
                    _StoryController.SetNode(Node.NextNode);
                }
            }
            else
            {
                View.Hide();
            }
		}

		private void HandleShowComplete()
		{
			
		}
		
		private void HandleHideComplete()
		{
			_StoryController.SetNode(Node.NextNode);
		}
	}
}
