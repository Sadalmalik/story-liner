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

		public List<BaseNode> nodes = new();

		[NonSerialized] public Dictionary<string, BaseNode> nodesByID = new();


		public void AddNode(BaseNode node)
		{
			nodes.Add(node);
			nodesByID.Add(node.id, node);
		}

		public void RemoveNode(string nodeId)
		{
			nodes.Remove(nodesByID[nodeId]);
			nodesByID.Remove(nodeId);
		}

		public void RemoveNode(BaseNode node)
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
				if (node != null)
					nodesByID.Add(node.id, node);
		}
	}
}