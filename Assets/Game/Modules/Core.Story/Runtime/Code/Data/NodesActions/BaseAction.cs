using System;
using UnityEngine;

namespace Self.Story
{
	public abstract class BaseAction : ScriptableObject, ICloneable
	{
		[DisplayOnNode(0)] public bool executeOnce;

		public abstract void Execute(BaseNode node);

		public object Clone()
		{
			var clone = ScriptableObject.Instantiate(this);
			clone.name = (string)this.name.Clone();
			return clone;
		}
	}
}