using System;

namespace Self.Architecture.Utils
{
	public static class BootExtensions
	{
		public static void Trigger(this ref bool value, Action callback)
		{
			if (value)
			{
				value = false;
				callback?.Invoke();
			}
		}
	}
}