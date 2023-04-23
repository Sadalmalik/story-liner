using UnityEngine;
using UnityEngine.UI;

namespace Self.Game
{
	public class GameCloser : MonoBehaviour
	{
		public Button button;

		public void Awake()
		{
			button.onClick.AddListener(HandleButtonClick);
		}

		private void HandleButtonClick()
		{
			Application.Quit();
		}
	}
}