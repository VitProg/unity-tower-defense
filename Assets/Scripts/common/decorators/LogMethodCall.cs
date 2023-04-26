using System;
using UnityEngine;

namespace td.common.decorators
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogMethodCall : Attribute
    {
        public void PerformCall(Action action)
        {
            Debug.Log($@"> call {action.Method.Name} {action.Method.GetType()}...");
            action.Invoke();
            Debug.Log($@"> end {action.Method.Name} {action.Method.GetType()}");
        }
        
        
    }
}