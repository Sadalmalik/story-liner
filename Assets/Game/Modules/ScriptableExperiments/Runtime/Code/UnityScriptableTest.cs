using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Self.Game.Experiments
{
	public class SubObject : ScriptableObject
	{
		public int value;
	}

	public class UnityScriptableTest : ScriptableObject
	{
		public List<SubObject> objects;

	}
}