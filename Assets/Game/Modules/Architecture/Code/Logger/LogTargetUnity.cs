using UnityEngine;

namespace Self.Architecture.Logger
{
    public class LogTargetUnity : LogTarget
    {
        public override void LogTemp(string message)
        {
            Debug.Log(message);
        }

        public override void LogInfo(string message)
        {
            Debug.Log(message);
        }

        public override void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public override void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}