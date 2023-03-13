
using System;
using UnityEngine;

namespace Self.Story
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