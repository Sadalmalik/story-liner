using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Self.Story
{
	[Serializable]
	public class ClueData
	{
		public string ClueText;
		public string[] TargetWords;
		public bool IsUnlocked;
	}

	[Serializable]
	public class Clue
	{
		public string[] TargetWords;

		public bool IsUnlocked { get; private set; }

		private RectTransform clueSlot;
		private CanvasGroup clueAlpha;



		public Clue(ClueData data, RectTransform clueSlot)
		{
			this.TargetWords = data.TargetWords;
			this.clueSlot = clueSlot;
			this.clueAlpha = clueSlot.GetComponentInChildren<CanvasGroup>();
			IsUnlocked = data.IsUnlocked;

			if(IsUnlocked)
			{
				clueAlpha.alpha = IsUnlocked ? 1f : 0f;
			}
		}

		public bool IsWithinBounds(Vector3 position)
		{
			var root = clueSlot.parent.GetComponent<RectTransform>();
			var rectRoot = new Rect(clueSlot.anchoredPosition.x, clueSlot.anchoredPosition.y, 0f, 0f);

			rectRoot.x += clueSlot.rect.x + root.rect.x;
			rectRoot.y += clueSlot.rect.y + root.rect.y;
			rectRoot.width = clueSlot.rect.width;
			rectRoot.height = clueSlot.rect.height;

			return rectRoot.Contains(position);
		}

		public bool TryUnlock(string word)
		{
			if (TargetWords.Contains(word))
			{
				if (IsUnlocked)
					return false;

				IsUnlocked = true;
				clueAlpha.alpha = 1f;
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	// Container to display clues
	public class CluesWidget : AnimatedWidget
	{
		public event Action onAllCluesSolved;

		[SerializeField] TextForCluesContainer textContainerForClues;
		[SerializeField] TextWidget textWidget;
		[SerializeField] RectTransform[] clueSlots;
		[SerializeField] TextMeshProUGUI[] clueTextContainers;

		Clue[] clues;



		public void SetClues(ClueData[] clueData)
		{
			clues = new Clue[clueData.Length];

			for (int i = 0; i < clueData.Length; i++)
			{
				var clue = new Clue(clueData[i], clueSlots[i]);
				clues[i] = clue;
				clueTextContainers[i].text = clueData[i].ClueText;
			}
		}

		public (bool, string) TryUnlock(Vector3 position, string word)
		{
			var tryWord = clues.FirstOrDefault(w => w.IsWithinBounds(position));

			if (tryWord != null)
			{
				var success = tryWord.TryUnlock(word);

				if (success)
				{
					if (!clues.Any(c => !c.IsUnlocked))
					{
						onAllCluesSolved?.Invoke();
					}
				}

				return (success, "area-hit");
			}
			else
			{
				return (false, "area-not-hit");
			}
		}

		public void InitClues(Clue[] clues)
		{
			this.clues = (Clue[])clues.Clone();
		}

		private void Awake()
		{
			textWidget.onTextChanged += HandleTextChanged;
		}

		private void HandleTextChanged(string text)
		{
			textContainerForClues.InitText(text);
		}
	}
}