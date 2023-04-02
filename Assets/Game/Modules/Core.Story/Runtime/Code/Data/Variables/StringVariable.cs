using System;

namespace Self.Story
{
	public class StringVariable : Variable
	{
		public string value;


		public override object GetValue()
		{
			return value;
		}

		public override void SetValue(object value)
		{
			if (value == null)
			{
				this.value = string.Empty;
			}

			try
			{
				var converted = Convert.ToString(value);
			}
			catch (Exception)
			{
				throw new InvalidCastException(
					$"[{nameof(StringVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(string)}");
			}
		}
	}
}