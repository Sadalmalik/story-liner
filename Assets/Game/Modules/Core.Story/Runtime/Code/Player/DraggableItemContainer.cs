using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Self.Story
{
	public class DraggableItemContainer : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		public string Id;

		public RectTransform RectTransform { get; set; }

		public UnityEvent onBeginDrag;
		public UnityEvent onDragFailed;
		public UnityEvent onDragSucceeded;

		private DragItemsWidget widget;
		private Vector3 initialPosition;
		private bool isDraggable;



		public void Init(DragItemsWidget widget)
		{
			this.widget = widget;
			RectTransform = GetComponent<RectTransform>();
			initialPosition = RectTransform.anchoredPosition;
			isDraggable = true;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			onBeginDrag?.Invoke();
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!isDraggable)
				return;

			RectTransform.anchoredPosition = eventData.position;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!isDraggable)
				return;

			if (!widget.TryDrag(this))
			{
				RectTransform.anchoredPosition = initialPosition;
				onDragSucceeded?.Invoke();
			}
			else
			{
				isDraggable = false;
				onDragFailed?.Invoke();
			}
		}
	}
}