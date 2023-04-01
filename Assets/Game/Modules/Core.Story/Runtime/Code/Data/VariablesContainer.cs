using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Self.Story
{
	public class VariablesContainer : ScriptableObject, ISerializationCallbackReceiver
	{
#region Data

		[SerializeField]
		private List<Variable> _variables;
		public  List<Variable> Variables => _variables;

		[field: NonSerialized] public Dictionary<string, Variable> VariablesById { get; private set; }

		public VariablesContainer()
		{
			_variables   = new List<Variable>();
			VariablesById = new Dictionary<string, Variable>();
		}

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
			foreach (var variable in Variables)
				VariablesById.Add(variable.id, variable);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			// Do nothing
		}

#endregion
	}
}