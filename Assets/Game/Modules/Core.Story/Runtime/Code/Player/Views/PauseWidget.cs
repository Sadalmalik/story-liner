using System;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class PauseWidget : MonoBehaviour
	{
		public Button button;

		public event Action OnClick;
		
		public void Awake()
		{
			if (button != null)
			{
				button.onClick.AddListener(HandleClick);
				button.interactable = false;
				button.gameObject.SetActive(false);
			}
		}

		public void Show()
		{
			button.interactable = true;
			button.gameObject.SetActive(true);
		}

		private void HandleClick()
		{
			OnClick?.Invoke();
			button.interactable = false;
			button.gameObject.SetActive(false);
		}
	}
}