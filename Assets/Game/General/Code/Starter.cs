using Self.Architecture.IOC;
using Self.Story;
using UnityEngine;

namespace Self.Game
{
	public class Starter : MonoBehaviour
	{
		private Container container;
		
		void Start()
		{
			container = Container.Global;

			container.Add<StoryManager>();
			
			container.Init();
		}

		private void OnApplicationQuit()
		{
			Debug.Log("Application ending after " + Time.time + " seconds");
			container.Dispose();
		}
	}

}