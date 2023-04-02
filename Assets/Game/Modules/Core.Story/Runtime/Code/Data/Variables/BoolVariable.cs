using System;

namespace Self.Story
{
	public class BoolVariable : Variable
	{
		public bool value;

		public override object GetValue()
		{
			return value;
		}

		public override void SetValue(object value)
		{
			if (value == null)
			{
				throw new Exception($"[{nameof(BoolVariable)}.SetValue] Tryint to set a null value as bool!");
			}

			try
			{
				var convertedValue = Convert.ToBoolean(value);

				this.value = convertedValue;
			}
			catch (Exception)
			{
				throw new InvalidCastException(
					$"[{nameof(BoolVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(bool)}");
			}
		}
	}
}