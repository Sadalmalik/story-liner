using System;
using System.Linq;
using UnityEngine;

namespace Self.Story
{
	[Serializable]
	public class DragZone
	{
		public string Id;
		public RectTransform Container;
	}

	public class DragItemsWidget : MonoBehaviour
	{
		public event Action<string, string> OnItemDragged;

		public DraggableItemContainer[] Items;
		public DragZone[] Zones;



		public void ShowItems()
		{
			foreach (var item in Items)
			{
				item.gameObject.SetActive(true);
				item.Init(this);
			}

			foreach (var zone in Zones)
			{
				zone.Container.gameObject.SetActive(true);
			}
		}

		public bool TryDrag(DraggableItemContainer draggableItemContainer)
		{
			var zone = Zones.FirstOrDefault(z => IsWithinBounds(z.Container, draggableItemContainer.RectTransform.anchoredPosition));

			if(zone != null)
			{
				OnItemDragged?.Invoke(draggableItemContainer.Id, zone.Id);

				return true;
			}
			else
			{
				return false;
			}
		}

		private bool IsWithinBounds(RectTransform container, Vector2 anchoredPosition)
		{
			return false;
		}
	}
}