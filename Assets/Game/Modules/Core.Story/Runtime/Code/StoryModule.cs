using GeekyHouse.Architecture.Events;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using UnityEngine;

namespace Self.Story
{
	public struct SStoryModuleReady
	{
		public StoryView view;
	}

	public class StoryModule : SharedObject
	{
		public static void Init(Container container)
		{
			container.Add<StoryController>();

			container.Add<NodeBaseController>();
			container.Add<NodeReplicaController>();
			container.Add<NodeChoiceController>();
		}

		public StoryModuleSettings Settings { get; private set; }

		public StoryView StoryView { get; private set; }

		public override void Init()
		{
			Settings = StoryModuleSettings.Instance;

			UnityEventProvider.OnNextUpdate += InstantiateStoryView; 
		}

		public override void Dispose()
		{
		}

		private void InstantiateStoryView()
		{
			StoryView = GameObject.Instantiate(Settings.prefab);

			SignalBus.Global.Invoke(new SStoryModuleReady {view = StoryView});
		}
	}
}