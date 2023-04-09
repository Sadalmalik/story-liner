using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;

namespace Self.Story
{
	public class NodeReplicaController : SharedObject, INodeController
	{
		public Type TargetType => typeof(ReplicaNode);

		private ReplicaWidget _view;
		
		public void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			_view = signal.view.replicaWidget;
		}
		
		public void Enter(
			BaseNode       node,
			Action<string> onNextCallback)
		{
			onNextCallback(node.nextNodes[0]);
		}

		public void Exit()
		{
		}
	}
}