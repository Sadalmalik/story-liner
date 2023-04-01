using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;
using UnityEditor.UIElements;
using System;

namespace Self.Story.Editors
{
	public class FieldData : IComparable<FieldData>
	{
		public int       order;
		public FieldInfo field;

		public int CompareTo(FieldData other)
		{
			return order - other.order;
		}
	}

	[CustomPropertyDrawer(typeof(NodeAction), true)]
	public class NodeActionPropertyDrawer : PropertyDrawer
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var serializedObject = new SerializedObject(property.objectReferenceValue);
			var container        = new VisualElement() {name = property.displayName};

			var fields = property.objectReferenceValue
				.GetType()
				.GetFields(BINDING_FLAGS)
				.Where(f => f.GetCustomAttribute(typeof(DisplayOnNodeAttribute)) != null)
				.Select(f => new FieldData
				{
					order = ((DisplayOnNodeAttribute) f.GetCustomAttribute(typeof(DisplayOnNodeAttribute))).order,
					field = f
				})
				.ToList();

			fields.Sort();

			foreach (var f in fields)
			{
				var field = serializedObject.FindProperty(f.field.Name);

				if (field != null)
				{
					var propertyFieldContainer = new PropertyField(field);
					propertyFieldContainer.BindProperty(serializedObject);

					container.Add(propertyFieldContainer);
				}
			}

			if (fields == null || fields.Count() == 0)
				return base.CreatePropertyGUI(property);
			else
				return container;
		}
	}
}