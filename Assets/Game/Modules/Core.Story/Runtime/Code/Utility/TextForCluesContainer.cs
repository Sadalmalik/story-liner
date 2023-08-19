using Self.Story;
using TMPro;
using UnityEngine;

public class TextForCluesContainer : MonoBehaviour
{
	[SerializeField] CluesWidget textDragArea;

	[SerializeField] TextMeshProUGUI textContainer;
	[SerializeField] TextMeshProUGUI highlighTextContainer;
	[SerializeField] CanvasGroup highlightTextCanvasGroup;
	[SerializeField] Material pullMaterial;

	[SerializeField] Vector3 highlightOffset;
	[SerializeField] string highlightedTextMaterial;

	TMP_TextInfo textInfo;
	TMP_WordInfo[] wordInfos;
	string currentWord;



	public void InitText(string text)
	{
		if(string.IsNullOrEmpty(text))
		{
			textContainer.ForceMeshUpdate(false, true);
			textInfo = null;
			wordInfos = null;
			return;
		}

		textContainer.text = text;
		textContainer.ForceMeshUpdate(false, true);
		textInfo = textContainer.GetTextInfo(text);
		textInfo.wordInfo = new TMP_WordInfo[16];
		textInfo.Clear();
		textInfo.ClearAllMeshInfo();
		textInfo.ClearMeshInfo(true);
		textInfo.ClearLineInfo();

		wordInfos = textInfo.wordInfo;
	}

	private void Awake()
	{
		highlightTextCanvasGroup.alpha = 0f;
	}

	private void Update()
	{
		if (wordInfos == null || textInfo == null)
			return;

		var mousePosition = Input.mousePosition;
		var p = transform.position;

		if(Input.GetMouseButtonUp(0))
		{
			if(string.IsNullOrEmpty(currentWord))
			{
				return;
			}

			var result = textDragArea.TryUnlock(mousePosition, currentWord);

			Debug.Log($"Tried unlocking: '{currentWord}', result: '{result}'");
		}

		pullMaterial.SetVector("_CursorPosition", mousePosition);

		if (!Input.GetMouseButton(0))
		{
			var isWithinAnyBounds = false;

			foreach (var word in wordInfos)
			{
				if (word.characterCount == 0)
					continue;

				var firstCharIndex = word.firstCharacterIndex;
				var lastCharIndex = word.lastCharacterIndex;

				var char0 = textInfo.characterInfo[firstCharIndex];
				var char1 = textInfo.characterInfo[lastCharIndex];

				var bottomLeft = char0.bottomLeft;
				var bottomRight = char1.bottomRight;
				var topLeft = char0.topLeft;
				var topRight = char1.topRight;

				var trueMinimum = GetMinimum(bottomLeft, bottomRight);
				var trueMaximum = GetMaximum(topLeft, topRight);

				if (IsWithinBounds(mousePosition, p + trueMinimum, p + trueMaximum))
				{
					isWithinAnyBounds = true;

					var wordString = word.GetWord();

					if(highlighTextContainer.text != wordString)
					{
						highlighTextContainer.text = WrapText(wordString, "material", highlightedTextMaterial);
					}

					highlighTextContainer.transform.position = p + bottomLeft + highlightOffset;
				}
			}

			if(!isWithinAnyBounds)
			{
				highlighTextContainer.text = string.Empty;
			}

			highlightTextCanvasGroup.alpha = isWithinAnyBounds ? 1f : 0f;
		}
		else
		{
			if(!string.IsNullOrEmpty(highlighTextContainer.text))
			{
				highlighTextContainer.transform.position = mousePosition;
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			foreach (var word in wordInfos)
			{
				if (word.characterCount == 0)
					continue;

				var firstCharIndex = word.firstCharacterIndex;
				var lastCharIndex = word.lastCharacterIndex;

				var char0 = textInfo.characterInfo[firstCharIndex];
				var char1 = textInfo.characterInfo[lastCharIndex];

				var bottomLeft = char0.bottomLeft;
				var bottomRight = char1.bottomRight;
				var topLeft = char0.topLeft;
				var topRight = char1.topRight;

				var trueMinimum = GetMinimum(bottomLeft, bottomRight);
				var trueMaximum = GetMaximum(topLeft, topRight);

				if (IsWithinBounds(mousePosition, p + trueMinimum, p + trueMaximum))
				{
					currentWord = word.GetWord();

					Debug.Log($"Clicked '{currentWord}'");
				}
			}
		}
	}

	private string WrapText(string text, string tag, string data)
	{
		return $"<{tag}={data}>{text}</{tag}>";
	}

	private Vector3 GetMinimum(Vector3 bottomLeft, Vector3 bottomRight)
	{
		var trueMinY = Mathf.Min(bottomLeft.y, bottomRight.y);

		return new Vector3(bottomLeft.x, trueMinY);
	}

	private Vector3 GetMaximum(Vector3 topLeft, Vector3 topRight)
	{
		var trueMaxY = Mathf.Max(topLeft.y, topRight.y);

		return new Vector3(topRight.x, trueMaxY);
	}

	private void OnDrawGizmosSelected()
	{
		if (textContainer == null)
			return;

		var textInfo = textContainer.textInfo;
		var words = textInfo.wordInfo;
		var p = transform.position;

		foreach (var word in words)
		{
			if (word.characterCount == 0)
				continue;

			var firstCharIndex = word.firstCharacterIndex;
			var lastCharIndex = word.lastCharacterIndex;

			var char0 = textInfo.characterInfo[firstCharIndex];
			var char1 = textInfo.characterInfo[lastCharIndex];

			var bottomLeft = char0.bottomLeft;
			var bottomRight = char1.bottomRight;
			var topLeft = char0.topLeft;
			var topRight = char1.topRight;

			var trueMinimum = GetMinimum(bottomLeft, bottomRight);
			var trueMaximum = GetMaximum(topLeft, topRight);

			topLeft.y = trueMaximum.y;
			bottomRight.y = trueMinimum.y;

			Gizmos.DrawLine(p + trueMinimum, p + topLeft);
			Gizmos.DrawLine(p + topLeft, p + trueMaximum);
			Gizmos.DrawLine(p + trueMaximum, p + bottomRight);
			Gizmos.DrawLine(p + bottomRight, p + trueMinimum);
		}
	}

	private bool IsWithinBounds(Vector3 position, Vector3 bottomLeft, Vector3 topRight)
	{
		var b1 = position.x > bottomLeft.x && position.y > bottomLeft.y;
		var t1 = position.x < topRight.x && position.y < topRight.y;

		return b1 && t1;
	}
}
