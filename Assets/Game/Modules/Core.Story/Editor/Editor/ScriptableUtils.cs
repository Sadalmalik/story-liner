using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Self.Story.Editors
{
	public static class ScriptableUtils
	{
		public static T Create<T>(string name = null) where T : ScriptableObject
		{
			var so = (T) ScriptableObject.CreateInstance(typeof(T));
			so.name = name ?? typeof(T).Name;
			return so;
		}
		
		public static T CreateAsset<T>(string path, string name = null, bool save = false) where T : ScriptableObject
		{
			var so = Create<T>(name);
			AssetDatabase.CreateAsset(so, Path.Combine(path, $"{so.name}.asset"));
			if (save) AssetDatabase.SaveAssets();
			return so;
		}

		public static T AddToAsset<T>(ScriptableObject parent, string name = null, bool save = false) where T : ScriptableObject
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));

			T so = Create<T>(name);
			
			var assetPath = AssetDatabase.GetAssetPath(parent);
			AssetDatabase.AddObjectToAsset(so, assetPath);
			EditorUtility.SetDirty(parent);
			
			if (save) AssetDatabase.SaveAssets();

			return so;
		}
	}
}