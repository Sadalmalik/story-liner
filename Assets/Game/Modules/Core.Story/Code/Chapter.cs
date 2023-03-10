using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
    [System.Serializable]
    public class NodePair
    {
        public string id;
        public Node node;
    }

    [System.Serializable]
    public class VariablesContainer
    {
        public Dictionary<string, float> floatVariables;
        public Dictionary<string, int> intVariables;
        public Dictionary<string, bool> boolVariables;
        public Dictionary<string, string> stringVariables;

        [SerializeReference] public List<FloatVariableData> serializedFloatVariables;
        public List<IntVariableData> serializedIntVariables;
        public List<BoolVariableData> serializedBoolVariables;
        public List<StringVariableData> serializedStringVariables;
    }

    [CreateAssetMenu]
	public class Chapter : ScriptableObject, ISerializationCallbackReceiver
	{
		public string chapterName;
		public string startNode;

		public Dictionary<string, Node> nodes;

        [SerializeField] private VariablesContainer variablesContainer;
        [SerializeField, HideInInspector] private List<NodePair> _serializedNodes;



        public bool TryGetVariable(string id, VariableType type, out object variable)
        {
            switch (type)
            {
                case VariableType.Float:
                    if (variablesContainer.floatVariables.ContainsKey(id))
                    {
                        variable = variablesContainer.floatVariables[id];
                        return true;
                    }
                    else
                    {
                        variable = default(float);
                        return false;
                    }
                case VariableType.Int:
                    if (variablesContainer.intVariables.ContainsKey(id))
                    {
                        variable = variablesContainer.intVariables[id];
                        return true;
                    }
                    else
                    {
                        variable = default(int);
                        return false;
                    }
                case VariableType.Bool:
                    if (variablesContainer.boolVariables.ContainsKey(id))
                    {
                        variable = variablesContainer.boolVariables[id];
                        return true;
                    }
                    else
                    {
                        variable = default(bool);
                        return false;
                    }
                case VariableType.String:
                    if (variablesContainer.stringVariables.ContainsKey(id))
                    {
                        variable = variablesContainer.stringVariables[id];
                        return true;
                    }
                    else
                    {
                        variable = default(string);
                        return false;
                    }
                default:
                    variable = default;
                    return false;
            }
        }

        public void OnAfterDeserialize()
        {
            DeserializeNodes();
            DeserializeVariableData(variablesContainer.serializedFloatVariables, ref variablesContainer.floatVariables);
            DeserializeVariableData(variablesContainer.serializedIntVariables, ref variablesContainer.intVariables);
            DeserializeVariableData(variablesContainer.serializedBoolVariables, ref variablesContainer.boolVariables);
            DeserializeVariableData(variablesContainer.serializedStringVariables, ref variablesContainer.stringVariables);
        }

        public void OnBeforeSerialize()
        {
            SerializeNodes();
            SerializeVariableData(variablesContainer.floatVariables, ref variablesContainer.serializedFloatVariables);
            SerializeVariableData(variablesContainer.intVariables, ref variablesContainer.serializedIntVariables);
            SerializeVariableData(variablesContainer.boolVariables, ref variablesContainer.serializedBoolVariables);
            SerializeVariableData(variablesContainer.stringVariables, ref variablesContainer.serializedStringVariables);
        }

        private void DeserializeNodes()
        {
            nodes = new Dictionary<string, Node>();

            if (_serializedNodes != null)
            {
                foreach (var np in _serializedNodes)
                {
                    nodes.Add(np.id, np.node);
                }
            }
        }

        private void DeserializeVariableData<T, U>(List<U> serializedData, ref Dictionary<string, T> deserializedData) where U : VariableData<T>, new()
        {
            deserializedData = new Dictionary<string, T>();

            if (serializedData != null)
            {
                foreach (var v in serializedData)
                {
                    deserializedData.Add(v.id, v.value);
                }
            }
        }

        private void SerializeNodes()
        {
            _serializedNodes = new List<NodePair>();

            if (nodes != null)
            {
                foreach (var np in nodes)
                {
                    _serializedNodes.Add(new NodePair
                    {
                        id = np.Key,
                        node = np.Value
                    });
                }
            }

        }

        private void SerializeVariableData<T, U>(Dictionary<string, T> dataToSerialize, ref List<U> serializedData) where U : VariableData<T>, new()
        {
            serializedData = new List<U>();

            if (dataToSerialize != null)
            {
                foreach (var v in dataToSerialize)
                {
                    serializedData.Add(new U
                    {
                        id = v.Key,
                        value = v.Value
                    });
                }
            }
        }
    }
}