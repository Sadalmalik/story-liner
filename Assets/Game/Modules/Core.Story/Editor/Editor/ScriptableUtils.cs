using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Self.Story.Editors
{
	public static class ScriptableUtils
	{
		public static T Create<T>(
			string name = null)
			where T : ScriptableObject
		{
			return (T) Create(typeof(T), name);
		}

		public static object Create(
			Type   type,
			string name = null)
		{
			var so = ScriptableObject.CreateInstance(type);
			so.name = name ?? type.Name;
			return so;
		}

		public static T CreateAsset<T>(
			string path,
			string name = null,
			bool   save = false)
			where T : ScriptableObject
		{
			return (T) CreateAsset(typeof(T), path, name, save);
		}

		public static Object CreateAsset(
			Type   type,
			string path,
			string name = null,
			bool   save = false)
		{
			var so = (Object) Create(type, name);
			AssetDatabase.CreateAsset(so, Path.Combine(path, $"{so.name}.asset"));
			if (save) AssetDatabase.SaveAssets();
			return so;
		}

		public static T AddToAsset<T>(
			Object parent,
			string name = null,
			bool   save = false)
			where T : ScriptableObject
		{
			return (T) AddToAsset(typeof(T), parent, name, save);
		}

		public static Object AddToAsset(
			Type   type,
			Object parent,
			string name = null,
			bool   save = false)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			var so = (Object) Create(type, name);

			var assetPath = AssetDatabase.GetAssetPath(parent);
			AssetDatabase.AddObjectToAsset(so, assetPath);
			EditorUtility.SetDirty(parent);

			if (save) AssetDatabase.SaveAssets();

			return so;
		}
	}
}