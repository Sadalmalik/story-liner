using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Self.StoryV2
{
    public class VariablesContainer : ScriptableObject
    {
        public List<Variable> variables;

        public Dictionary<string, Variable> variablesById;



        public void Init()
        {
            variablesById = new Dictionary<string, Variable>();

            foreach (var v in variables)
            {
                variablesById.Add(v.id, v);
            }
        }

        public Variable Get(string id)
        {
            if(variablesById != null)
            {
                if (variablesById.TryGetValue(id, out var value))
                    return value;
            }

            if(variables.Any(v => v.id == id))
            {
                return variables.First(v => v.id == id);
            }

            return null;
        }

        public object GetValue(string id)
        {
            var variable = Get(id);

            if (variable != null)
                return variable.GetValue();

            return default;
        }
    }
}