using UnityEngine;

namespace Self.Game
{
	[ExecuteInEditMode]
	public class TestParse : MonoBehaviour
	{
		public string value;
		public int    int_value;
		public bool   bool_value;
		public bool   trigger;

		public void Update()
		{
			if (trigger)
			{
				trigger = false;

				int_value = 0;
				int.TryParse(value, out int_value);

				bool_value = false;
				bool.TryParse(value, out bool_value);
			}
		}
	}
}