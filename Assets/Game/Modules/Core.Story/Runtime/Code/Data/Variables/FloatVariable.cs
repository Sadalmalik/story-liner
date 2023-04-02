using System;
using UnityEngine;

namespace Self.Story
{
	public class FloatVariable : Variable
	{
		public float value;
		public float minValue = 0f;
		public float maxValue = 100f;

		public override object GetValue()
		{
			return value;
		}

		public override void SetValue(object value)
		{
			if (value == null)
				throw new Exception($"[{nameof(FloatVariable)}.SetValue] Tryint to set a null value as float!");

			try
			{
				var convertedValue = Convert.ToSingle(value);

				this.value = Mathf.Clamp(convertedValue, minValue, maxValue);
			}
			catch (Exception)
			{
				throw new InvalidCastException(
					$"[{nameof(FloatVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(float)}");
			}
		}
	}
}