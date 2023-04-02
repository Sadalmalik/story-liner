using System;
using UnityEngine;

namespace Self.Story
{
	public class IntVariable : Variable
	{
		public int value;
		public int minValue = 0;
		public int maxValue = 100;

		public override object GetValue()
		{
			return value;
		}

		public override void SetValue(object value)
		{
			if (value == null)
				throw new Exception($"[{nameof(IntVariable)}.SetValue] Tryint to set a null value as int!");
			try
			{
				var convertedValue = Convert.ToInt32(value);

				this.value = Mathf.Clamp(convertedValue, minValue, maxValue);
			}
			catch (Exception)
			{
				throw new InvalidCastException(
					$"[{nameof(IntVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(int)}");
			}
		}
	}
}