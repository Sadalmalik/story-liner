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

    [CreateAssetMenu]
	public class Chapter : ScriptableObject, ISerializationCallbackReceiver
	{
		public string chapterName;
		public string startNode;

		public Dictionary<string, Node> nodes;

        [SerializeField, HideInInspector] private List<NodePair> _serializedNodes;



        public void OnAfterDeserialize()
        {
            nodes = new Dictionary<string, Node>();

            if(_serializedNodes != null)
            {
                foreach (var np in _serializedNodes)
                {
                    nodes.Add(np.id, np.node);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            _serializedNodes = new List<NodePair>();

            if(nodes != null)
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
    }
}