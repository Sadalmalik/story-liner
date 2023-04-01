using Self.Story;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.Story
{
	[CreateAssetMenu]
	public class Chapter : ScriptableObject, ISerializationCallbackReceiver
	{
		public Book book;

		public string chapterName;
		public string startNodeID;

		public List<Node> nodes;

		[NonSerialized] public Dictionary<string, Node> nodesByID;

		public void AddNode(Node node)
		{
			nodes.Add(node);
			nodesByID.Add(node.id, node);
		}
		public void RemoveNode(Node node)
		{
			nodes.Remove(node);
			nodesByID.Remove(node.id);
		}
		
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			nodesByID = new Dictionary<string, Node>();
			foreach (var node in nodes)
				nodesByID.Add(node.id, node);
		}
	}
}