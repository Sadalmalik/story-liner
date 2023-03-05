using System;

namespace Self.Architecture.Logger
{
	public class LogTargetObservable : LogTarget
	{
		public event Action<string> logAll;

		public event Action<string> logTemp;

		public event Action<string> logInfo;

		public event Action<string> logWarning;

		public event Action<string> logError;

		public override void LogTemp(string message)
		{
			logAll?.Invoke(message);
			logTemp?.Invoke(message);
		}

		public override void LogInfo(string message)
		{
			logAll?.Invoke(message);
			logInfo?.Invoke(message);
		}

		public override void LogWarning(string message)
		{
			logAll?.Invoke(message);
			logWarning?.Invoke(message);
		}

		public override void LogError(string message)
		{
			logAll?.Invoke(message);
			logError?.Invoke(message);
		}
	}
}
