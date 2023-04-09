using System.Collections.Generic;
using Self.Architecture.Logger;
using UnityEngine;

namespace Self.Architecture.CSV
{
	[ExecuteInEditMode]
	public class TEST_OCM : MonoBehaviour
	{
		public class MyItem
		{
			public string name;
			public bool   boolValue;
			public int    intValue;
			public float  floatValue;
		}

		public List<MyItem> items1;
		public List<MyItem> items2;

		public bool doTests;
		public void Update()
		{
			if (doTests)
			{
				doTests = false;
				DoTests();
			}
		}
		public void DoTests()
		{
			var csv = ObjectCSVConverter.ToCSV(items1);
			
			Log.Temp($"CSV:\n\n{csv}");
			
			items2 = ObjectCSVConverter.FromCSV<MyItem>(csv);
		}

	}
}