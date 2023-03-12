using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SubObject : ScriptableObject
{
	public int value;
}

public class Azaza : ScriptableObject
{
	public List<SubObject> objects;

	[MenuItem("Assets/[SELF]/AZAZA", false, 1)]
	public static void CreateAzaza()
	{
		var azaza = ScriptableObject.CreateInstance<Azaza>();
		var path  = AssetDatabase.GetAssetPath (Selection.activeObject);
		ProjectWindowUtil.CreateAsset(azaza, Path.Combine(path, "azaza.asset"));
		AssetDatabase.SaveAssets();
		
	}

	[MenuItem("Assets/[SELF]/Fill AZAZA", false, 1)]
	public static void FillAzaza()
	{
		var azaza = Selection.activeObject as Azaza;
		if (azaza == null)
			return;
		
		azaza.objects = new List<SubObject>();
		for (int i = 0; i < 15; i++)
		{
			var zub = CreateInstance<SubObject>();
			AssetDatabase.AddObjectToAsset(zub, azaza);
			zub.name  = $"ZUB.{i}";
			zub.value = i;
			azaza.objects.Add(zub);
			EditorUtility.SetDirty(zub);
		}
		
		EditorUtility.SetDirty(azaza);
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Assets/[SELF]/Fill AZAZA 2", false, 1)]
	public static void FillAzaza2()
	{
		var azaza = Selection.activeObject as Azaza;
		if (azaza == null)
			return;
		
		azaza.objects = new List<SubObject>();

		void Create(string name)
		{
			var zub = CreateInstance<SubObject>();
			AssetDatabase.AddObjectToAsset(zub, azaza);
			zub.name  = name;
			zub.value = 0;
			azaza.objects.Add(zub);
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
		
		EditorUtility.SetDirty(azaza);
		AssetDatabase.SaveAssets();
	}
}
