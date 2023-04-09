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

			View.OnClick += HandleClick;
		}

		private void HandleClick()
		{
			_StoryController.SetNode(Node.NextNode);
		}

		public void Enter(
			BaseNode       node,
			Action<string> onNextCallback)
		{
			Node = node as ReplicaNode;
			View.SetNode(Node);
		}

		public void Exit()
		{
		}
	}
}