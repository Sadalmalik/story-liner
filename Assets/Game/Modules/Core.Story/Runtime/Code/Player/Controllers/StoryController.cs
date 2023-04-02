using System;
using Self.Architecture.DataStructures;
using Self.Architecture.IOC;
using UnityEngine;

namespace Self.Story
{
	public abstract class BaseStoryController : SharedObject
	{
		public TypeContainer<BaseNode, NodeBaseController>     controllers = new();
		public TypeContainer<BaseAction, ActionBaseController> actions     = new();

		public ChapterSave CurrentSave    { get; private set; }
		public Chapter     CurrentChapter { get; private set; }
		public BaseNode    CurrentNode    { get; private set; }
		public string      CurrentNodeID  { get; private set; }

		private NodeBaseController ActiveController;

		public event Action<BaseNode> OnNodeEnter;
		public event Action<string>   OnStoryBroken;

		public abstract override void Init();
		public abstract override void Dispose();
		
		private void HandleNext(string nextNode)
		{
			SetNode(nextNode);
		}

		public void SetChapter(Chapter chapter, ChapterSave save)
		{
			CurrentChapter = chapter;
			CurrentNodeID  = save.currentNode ?? chapter.startNodeID;
			CurrentNode    = CurrentChapter.nodesByID[CurrentNodeID];

			SetNode(CurrentNodeID);
		}

		public void SetNode(string nodeId)
		{
			if (!CurrentChapter.nodesByID.TryGetValue(nodeId, out var node))
			{
				Debug.LogError($"Story broken! No node with id '{nodeId}'");
				OnStoryBroken?.Invoke(CurrentNodeID);
				return;
			}

			ActiveController?.Exit();

			ActiveController = controllers.Get(node.GetType());
			
			if (ActiveController == null)
			{
				Debug.Log($"Story broken! No controller defined for node type '{node.GetType()}'");
				OnStoryBroken?.Invoke(CurrentNodeID);
				return;
			}
			
			// Так. Тут надо как-то прикрутить такую штуку как выполнение экшенов ПОСЛЕ запуска контроллера, но ДО вызова следующей ноды!!
			ActiveController.Enter(node, SetNode);
		}
	}
}