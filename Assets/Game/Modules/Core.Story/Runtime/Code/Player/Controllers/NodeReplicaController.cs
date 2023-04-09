using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ReplicaNode);

		[Inject] private StoryController _StoryController;

		public ReplicaNode   Node { get; private set; }
		public ReplicaWidget View { get; private set; }

		public void Init()
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
			View.Show(Node);

			return null;
		}
		
		private void HandleClick()
		{
			View.Hide();
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