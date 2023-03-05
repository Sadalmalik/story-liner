namespace Self.Architecture.Logger
{
    public abstract class LogTarget
    {
        public abstract void LogTemp(string message);

        public abstract void LogInfo(string message);

        public abstract void LogWarning(string message);

        public abstract void LogError(string message);
    }
}