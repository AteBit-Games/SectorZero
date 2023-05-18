using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.BehaviourTree.Actions.Debug 
{
    [Serializable]
    [Name("Log Key")]
    [Category("Debug")]
    [Description("Log a blackboard key value to the console")]
    public class LogKey : ActionNode 
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
        public NodeProperty messageKey = new();

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate() 
        {
            switch (logType) 
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(messageKey.reference);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(messageKey);
                    break;
                case LogType.Error:
                    UnityEngine.Debug.LogError(messageKey);
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
