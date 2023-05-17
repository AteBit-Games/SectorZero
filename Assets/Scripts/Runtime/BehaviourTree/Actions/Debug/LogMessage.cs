using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.BehaviourTree.Actions.Debug 
{
    [Serializable]
    [Name("Log Message")]
    [Category("Debug")]
    [Description("Log a message to the console")]
    public class LogMessage : ActionNode 
    {
        public enum LogType 
        {
            Log,
            Warning,
            Error
        }
        
        [Tooltip("Type of message to log")]
        public LogType logType = LogType.Log;
        
        [Tooltip("Message to log to the console")]
        public NodeProperty<string> message = new();

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            switch (logType) 
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(message.Value);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(message.Value);
                    break;
                case LogType.Error:
                    UnityEngine.Debug.LogError(message.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return State.Success;
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }
    }
}
