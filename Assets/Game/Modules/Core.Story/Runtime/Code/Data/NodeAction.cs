using System;
using UnityEngine;

namespace Self.StoryV2
{
	public abstract class NodeAction : ScriptableObject, ICloneable
	{
		public bool executeOnce;

		public abstract void OnExecute();

		public object Clone()
		{
			var clone = ScriptableObject.Instantiate(this);
			clone.name = this.name;
			return clone;
		}
	}
}