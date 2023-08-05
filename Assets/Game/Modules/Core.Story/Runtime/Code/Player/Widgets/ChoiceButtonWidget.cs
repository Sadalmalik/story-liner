using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Self.Story
{
	public class ChoiceButtonWidget : MonoBehaviour
	{
		public Button   button;
		public TMP_Text label;

		public event Action<int> OnClick;

		private int _index;

		public void Awake()
		{
			button.onClick.AddListener(HandleClick);
		}

		public void Init(int index, string text)
		{
			_index = index;
			label.SetText(text);
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
		
		private void HandleClick()
		{
			OnClick?.Invoke(_index);
		}
	}
}