using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	public class VariablesContainer : ScriptableObject, ISerializationCallbackReceiver
	{
#region Data

		[SerializeField] private List<Variable> _variables = new(); 

		public List<Variable> Variables => _variables;

		[field: NonSerialized] public Dictionary<string, Variable> VariablesById { get; private set; } = new();

#endregion


#region Variable Operations

		public void Add(Variable variable)
		{
			Variables.Add(variable);
			VariablesById.Add(variable.id, variable);
		}

		public void Remove(string id)
		{
			if (VariablesById.TryGetValue(id, out var variable))
				Variables.Remove(variable);
		}

		public void Remove(Variable variable)
		{
			Variables.Remove(variable);
			VariablesById.Remove(variable.id);
		}

		public Variable Get(string id)
		{
			if (VariablesById.TryGetValue(id, out var value))
				return value;

			return null;
		}

		public object GetValue(string id)
		{
			var variable = Get(id);

			if (variable != null)
				return variable.GetValue();

			return default;
		}

#endregion


#region ISerializationCallbackReceiver

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			VariablesById.Clear();
			foreach (var variable in Variables)
				if(variable.id != null)
					VariablesById.Add(variable.id, variable);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			// Do nothing
		}

#endregion
	}
}