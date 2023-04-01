using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Self.Game.Experiments
{
	public static class UnityScriptableTestEditor
	{
		[MenuItem("Assets/[SELF]/Unity Scriptable Test", false, 1)]
		public static void CreateAzaza()
		{
			var azaza = ScriptableObject.CreateInstance<UnityScriptableTest>();
			var path  = AssetDatabase.GetAssetPath(Selection.activeObject);
			ProjectWindowUtil.CreateAsset(azaza, Path.Combine(path, "azaza.asset"));
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Assets/[SELF]/Fill Unity Scriptable Test", false, 1)]
		public static void FillAzaza()
		{
			var test = Selection.activeObject as UnityScriptableTest;
			if (test == null)
				return;

			test.objects = new List<SubObject>();
			for (int i = 0; i < 15; i++)
			{
				var zub = ScriptableObject.CreateInstance<SubObject>();
				AssetDatabase.AddObjectToAsset(zub, test);
				zub.name  = $"ZUB.{i}";
				zub.value = i;
				test.objects.Add(zub);
				EditorUtility.SetDirty(zub);
			}

			EditorUtility.SetDirty(test);
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Assets/[SELF]/Fill Unity Scriptable Test 2", false, 1)]
		public static void FillAzaza2()
		{
			var test = Selection.activeObject as UnityScriptableTest;
			if (test == null)
				return;

			test.objects = new List<SubObject>();

			void Create(string name)
			{
				var zub = ScriptableObject.CreateInstance<SubObject>();
				AssetDatabase.AddObjectToAsset(zub, test);
				zub.name  = name;
				zub.value = 0;
				test.objects.Add(zub);
				EditorUtility.SetDirty(zub);
			}

			Create(".settings");
			Create(".variable.name01");
			Create(".variable.name02");
			Create(".variable.name03");
			Create("node.0.entry");
			Create("node.1.replica");
			Create("node.2.replica");
			Create("node.3.choice");
			Create("node.4.A.replica");
			Create("node.5.A.replica");
			Create("node.6.B.replica");
			Create("node.7.B.replica");
			Create("node.8.exit");

			EditorUtility.SetDirty(test);
			AssetDatabase.SaveAssets();
		}
	}
}