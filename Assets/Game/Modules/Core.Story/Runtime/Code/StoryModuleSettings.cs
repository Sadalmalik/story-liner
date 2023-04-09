using GeekyHouse.Architecture.Patterns;
using UnityEditor;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu(
		menuName = "[SELF]/Story/Module Settings",
		fileName = "StoryModuleSettings",
		order = 0)]
	public class StoryModuleSettings : SingletonScriptableObject<StoryModuleSettings>
	{
		public StoryView prefab;

		[Space] public Chapter testChapter;
	}
}