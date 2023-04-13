using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	public class VariablesContainer : ScriptableObject
	{
#region Data

		[SerializeField] private List<Variable> _variables = new(); 

		public List<Variable> Variables => _variables;

		private Dictionary<string, Variable> _variablesById;

		[field: NonSerialized]
		public Dictionary<string, Variable> VariablesById
		{
			get
			{
				if (_variablesById == null)
				{
					_variablesById = new();
					foreach (var variable in Variables)
						if(variable.id != null)
							_variablesById.Add(variable.id, variable);
				}

				return _variablesById;
			}
			
		}

#endregion


#region Variable Operations

		public void Add(Variable variable)
		{
			VariablesById.Add(variable.id, variable);
			Variables.Add(variable);
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
	}
}