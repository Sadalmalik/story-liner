using Self.Architecture.IOC;
using Self.Architecture.Signals;
using Self.Story;
using UnityEngine;

namespace Self.Game
{
	public class GameStarter : MonoBehaviour
	{
		private Container container;

		public Vector2Int startResolution = new Vector2Int(400, 800);
		
		void Start()
		{
			Screen.SetResolution(
				startResolution.x,
				startResolution.y,
				FullScreenMode.Windowed);
			container = Container.Global;

			/*
			 * Мысль: надо продумать более внятные абстракции для инициализации
			 * В том плане, что:
			 *   - У каждого модуля свой процесс инициализации
			 *   - Некоторые модули должны ждать инициализации других модулей
			 *   - Нужно как-то обрабатывать цепочки инициализаций
			 *   - Нужно подсчитывать общий прогресс загрузки
			 * В общем надо опписать систему асинхронных тасок в вакууме
			**/
			StoryModule.Init(container);
			
			container.Init();
		}

		private void OnApplicationQuit()
		{
			Debug.Log("Application ending after " + Time.time + " seconds");
			container.Dispose();
		}
	}

}