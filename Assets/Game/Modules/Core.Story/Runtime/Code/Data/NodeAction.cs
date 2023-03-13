using UnityEngine;

namespace Self.StoryV2
{
	public abstract class NodeAction : ScriptableObject
	{
		public bool executeOnce;

		public abstract void OnExecute();
	}
}