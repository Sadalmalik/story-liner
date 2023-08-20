using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Self.Story.Editors
{
	[InspectedType(typeof(DragItemNode))]
	[CustomEditor(typeof(DragItemNode))]
	public class DragItemNodeEditor : ActiveNodeEditor
	{
		private DragItemNode m_Node;
		private SerializedProperty m_ItemIdProperty;
		private SerializedProperty m_ZoneIdProperty;



		protected override void OnEnable()
		{
			base.OnEnable();

			m_Node = serializedObject.targetObject as DragItemNode;
			m_ItemIdProperty = serializedObject.FindProperty(nameof(DragItemNode.ItemId));
			m_ZoneIdProperty = serializedObject.FindProperty(nameof(DragItemNode.TargetZoneId));
		}

		protected override void CreateNodeGUI(VisualElement nodeGuiRoot)
		{
			CreatePlayButton(nodeGuiRoot);
			CreateDragItemNodeGui(nodeGuiRoot);
			CreateActionsContainerGUI(nodeGuiRoot);
		}

		private void CreateDragItemNodeGui(VisualElement nodeGuiRoot)
		{
			var itemIdTextContainer = new TextField(nameof(DragItemNode.ItemId));
			var zoneIdTextContainer = new TextField(nameof(DragItemNode.TargetZoneId));

			itemIdTextContainer.RegisterValueChangedCallback(HandleIdChanged);
			itemIdTextContainer.SetValueWithoutNotify(m_ItemIdProperty.stringValue);

			zoneIdTextContainer.RegisterValueChangedCallback(HandleZoneIdChanged);
			zoneIdTextContainer.SetValueWithoutNotify(m_ZoneIdProperty.stringValue);

			nodeGuiRoot.Add(itemIdTextContainer);
			nodeGuiRoot.Add(zoneIdTextContainer);
		}

		private void HandleIdChanged(ChangeEvent<string> evt)
		{
			if (evt.previousValue == evt.newValue)
				return;

			serializedObject.Update();

			m_ItemIdProperty.stringValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}

		private void HandleZoneIdChanged(ChangeEvent<string> evt)
		{
			if (evt.previousValue == evt.newValue)
				return;

			serializedObject.Update();

			m_ZoneIdProperty.stringValue = evt.newValue;
			serializedObject.ApplyModifiedProperties();
		}
	}
}