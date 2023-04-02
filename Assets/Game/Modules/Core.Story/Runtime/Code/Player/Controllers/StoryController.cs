using System;
using System.Collections.Generic;
using Self.Architecture.DataStructures;
using Self.Architecture.IOC;
using UnityEngine;

namespace Self.Story
{
	public class StoryController : SharedObject
	{
		private Dictionary<Type, NodeBaseController>   _controllersByNodeType = new();
		private Dictionary<Type, ActionBaseController> _actionsByNodeType     = new();

		private string _nextNodeID;
		private bool   _nodeInProgress;

		public ChapterSave CurrentSave    { get; private set; }
		public Chapter     CurrentChapter { get; private set; }
		public BaseNode    CurrentNode    { get; private set; }
		public string      CurrentNodeID  { get; private set; }

		public NodeBaseController ActiveController;

		public event Action<BaseNode> OnNodeEnter;
		public event Action<string>   OnStoryBroken;


		public override void Init()
		{
			var list = container.GetAll<NodeBaseController>();
			foreach (var controller in list)
				_controllersByNodeType.Add(controller.TargetType, controller);

			var actions = container.GetAll<ActionBaseController>();
			foreach (var action in actions)
				_actionsByNodeType.Add(action.TargetType, action);
		}

		public override void Dispose()
		{
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
				foreach (var actionData in activeNode.actions)
					if (_actionsByNodeType.TryGetValue(actionData.GetType(), out var action))
						action.Execute(node);

			_nodeInProgress = false;
			if (_nextNodeID != null)
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