using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace Self.Story.Editors
{
	[CustomPropertyDrawer(typeof(CharacterReference))]
	public class CharacterPropertyDrawer : PropertyDrawer
	{
		// TODO [Andrei]: Replace this with a link
		// to some editor config
		// for later linking to a chapter
		private const string CONTAINER_PATH = "Assets/Game/General/Temp/CharConfig_Chapter_001.asset";
		private       string m_PropertyName = "character";

		private Dictionary<string, CharacterArg> m_CharacterDropDownChoices;


		public class CharacterDisplayItem
		{
			public string    id;
			public Character character;
		}

		public class CharacterArg
		{
			public SerializedProperty   Property;
			public CharacterDisplayItem Id;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var idProp          = property.FindPropertyRelative(m_PropertyName);
			var displayPosition = EditorGUI.PrefixLabel(position, new GUIContent(label));

			var displayName = ((idProp.objectReferenceValue != null)
				? ((Character) idProp.objectReferenceValue).characterName
				: string.Empty);

			if (EditorGUI.DropdownButton(displayPosition, new GUIContent(displayName), FocusType.Keyboard))
			{
				DisplayItemsMenu(idProp);
			}
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var idProp = property.FindPropertyRelative(m_PropertyName);

			var choices = LoadItemIds(idProp);
			m_CharacterDropDownChoices = new Dictionary<string, CharacterArg>();

			foreach (var ch in choices)
			{
				m_CharacterDropDownChoices.Add(ch.id, new CharacterArg
				{
					Property = idProp,
					Id       = ch
				});
			}

			var characterNames    = choices.Select(ch => ch.character.characterName).ToList();
			var characterDropDown = new DropdownField("Main Behaviour", characterNames, 0, OnCharacterSelected);

			return characterDropDown;
		}

		private IEnumerable<CharacterDisplayItem> LoadItemIds(SerializedProperty property)
		{
			var config = AssetDatabase.LoadAssetAtPath<ChapterCharactersConfig>(CONTAINER_PATH);

			var ids = new List<CharacterDisplayItem>();

			foreach (var character in config.characters)
			{
				ids.Add(new CharacterDisplayItem
				{
					id        = character.characterName,
					character = character
				});
			}

			return ids;
		}

		private void DisplayItemsMenu(SerializedProperty property)
		{
			var items = LoadItemIds(property);

			GenericMenu menu = new GenericMenu();

			var currentId = property.objectReferenceValue;

			{
				var itemIdArg = new CharacterArg()
				{
					Property = property,
					Id       = null
				};

				menu.AddItem(new GUIContent("Empty String"), string.Equals(currentId, string.Empty), OnItemSelected,
					itemIdArg);
			}

			foreach (var item in items)
			{
				var itemIdArg = new CharacterArg()
				{
					Property = property,
					Id       = item
				};

				menu.AddItem(new GUIContent(item.id), string.Equals(currentId, item), OnItemSelected, itemIdArg);
			}

			menu.ShowAsContext();
		}

		private string OnCharacterSelected(string character)
		{
			var arg = m_CharacterDropDownChoices[character];
			arg.Property.objectReferenceValue = arg.Id.character;
			arg.Property.serializedObject.ApplyModifiedProperties();

			return character;
		}

		protected virtual void OnItemSelected(object item)
		{
			if (item == null)
				return;

			var itemIdProp = (CharacterArg) item;

			itemIdProp.Property.objectReferenceValue = itemIdProp.Id == null ? null : itemIdProp.Id.character;
			itemIdProp.Property.serializedObject.ApplyModifiedProperties();
		}
	}
}