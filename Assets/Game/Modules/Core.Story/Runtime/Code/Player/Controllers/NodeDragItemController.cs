using System;
using Self.Architecture.IOC;
using Self.Architecture.Signals;
using UnityEngine;

namespace Self.Story
{
	public class NodeDragItemController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(DragItemNode);

		[Inject] private StoryController _StoryController;

		public DragItemNode Node { get; private set; }
		public DragItemsWidget DragItemWidget { get; private set; }

		private string currentItemId;
		private string currentZoneId;



		public override void Init()
		{
			SignalBus.Global.Subscribe<SStoryModuleReady>(HandleLoadingComplete);
		}

		private void HandleLoadingComplete(SStoryModuleReady signal)
		{
			DragItemWidget = signal.view.dragItemsWidget;
		}

		public string Enter(BaseNode node, BaseNode previousNode = null)
		{
			Node = node as DragItemNode;

			var targetItem = Node.ItemId;
			var targetZone = Node.TargetZoneId;

			DragItemWidget.ShowItems();
			DragItemWidget.OnItemDragged += HandleItemDragged;

			return null;
		}

		private void HandleItemDragged(string itemId, string zoneId)
		{
			if (itemId == currentItemId && zoneId == currentZoneId)
			{
				_StoryController.SetNode(Node.NextNode);
			}
			else
			{
				Debug.Log($"Required item: '{currentItemId}' to '{currentZoneId}'");
				Debug.Log($"Dragged item: '{itemId}' to '{zoneId}'");
			}
		}
	}
}