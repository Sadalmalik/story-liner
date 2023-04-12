using GeekyHouse.Architecture.Events;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using UnityEngine;

namespace Self.Story
{
	public class StoryModule : SharedObject
	{
		public static void Init(Container container)
		{
			container.Add<StoryModule>();
			container.Add<StoryController>();

			container.Add<NodeBaseController>();
			container.Add<NodeReplicaController>();
			container.Add<NodeChoiceController>();
			container.Add<NodeEntryController>();
			container.Add<NodeExitController>();
			container.Add<NodeConditionController>();
		}

		[Inject] private StoryController _storyController;

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
			//StoryView = GameObject.Instantiate(Settings.prefab);

			StoryView = GameObject.FindObjectOfType<StoryView>();

			SignalBus.Global.Invoke(new SStoryModuleReady {view = StoryView});
			
			_storyController.SetChapter(Settings.testChapter, null);
		}
	}
}