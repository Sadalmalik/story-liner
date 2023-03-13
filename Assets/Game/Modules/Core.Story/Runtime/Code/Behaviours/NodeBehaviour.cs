
using System;
using UnityEngine;

namespace Self.Story
{
	public class NodeBehaviour : ScriptableObject, ICloneable
	{
		public virtual void OnFirstEnter()
		{
		}

		public virtual void OnEnter()
		{
		}

		public virtual void OnExit()
		{
		}

        public object Clone()
        {
			var clone = ScriptableObject.Instantiate(this);
			clone.name = this.name;
			return clone;
		}
    }
}