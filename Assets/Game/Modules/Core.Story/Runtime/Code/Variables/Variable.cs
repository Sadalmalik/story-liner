using UnityEngine;

namespace Self.Story
{
	public enum VariableType
	{
		Float,
		Int,
		Bool,
		String
	}

	public abstract class Variable : ScriptableObject
	{
		public string id;

		public abstract void   SetValue(object value);
		public abstract object GetValue();
	}
}