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

		public List<Node> nodes = new();

		[NonSerialized] public Dictionary<string, Node> nodesByID = new();

		
		
		public void AddNode(Node node)
		{
			nodes.Add(node);
			nodesByID.Add(node.id, node);
		}
		
		public void RemoveNode(string nodeId)
		{
			nodes.Remove(nodesByID[nodeId]);
			nodesByID.Remove(nodeId);
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
			nodesByID.Clear();
			foreach (var node in nodes)
				nodesByID.Add(node.id, node);
		}
	}
}