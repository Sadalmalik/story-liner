using System;
using System.Collections.Generic;
using Self.Architecture.DataStructures;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using UnityEngine;

namespace Self.Story
{
	public class StoryController : SharedObject
	{
		private Dictionary<Type, INodeController>   _controllersByNodeType = new();

		private string _nextNodeID;
		private bool   _nodeInProgress;

		public ChapterSave CurrentSave    { get; private set; }
		public Chapter     CurrentChapter { get; private set; }
		public BaseNode    CurrentNode    { get; private set; }
		public string      CurrentNodeID  { get; private set; }

		public INodeController ActiveController;

		public event Action<BaseNode> OnNodeEnter;
		public event Action<string>   OnStoryBroken;

		public StoryView StoryView { get; private set; }

		public override void Init()
		{
			var list = container.GetAll<INodeController>();
			foreach (var controller in list)
				_controllersByNodeType.Add(controller.TargetType, controller);
			
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		public override void Dispose()
		{
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			StoryView = signal.view;
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

			if (!_controllersByNodeType.TryGetValue(node.GetType(), out ActiveController))
			{
				Debug.Log($"Story broken! No controller defined for node type '{node.GetType()}'");
				OnStoryBroken?.Invoke(CurrentNodeID);
				return;
			}

			_nextNodeID     = null;
			_nodeInProgress = true;

			ActiveController.Enter(node, HandleNext);

			if (node is ActiveNode activeNode)
				foreach (var action in activeNode.actions)
				{
					// Ход конем - инжектим зависимости, что бы команды могли делать что угодно
					container.InjectAt(action);
					action.Execute(node);
				}

			_nodeInProgress = false;
			if (_nextNodeID != null)
				// Вроде как C# умеет в хвостовую рекурсию
				// Но возможно стоит переписать с рекурсии на цикл
				SetNode(_nextNodeID);
		}


		private void HandleNext(string nextNode)
		{
			_nextNodeID = nextNode;
			if (!_nodeInProgress)
				SetNode(_nextNodeID);
		}
	}
}