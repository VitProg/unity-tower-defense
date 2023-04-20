using System;
using Debug = UnityEngine.Debug;

namespace td.common.decorators
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : Attribute
    {
        public object CallMethod(Func<object> method)
        {
            // Write entry message.
            string entryMessage = $"{method.Method.Name} started.";
            Debug.Log(entryMessage);

            try
            {
                // Invoke the method and store the result in a variable.
                var result = method();

                // Display the success message. The message is different when the method is void.
                string successMessage = $"{method.Method.Name} ";

                if (method.Method.ReturnType == typeof(void))
                {
                    // When the method is void, display a constant text.
                    successMessage += "succeeded.";
                }
                else
                {
                    // When the method has a return value, add it to the message.
                    successMessage += $"returned {result}.";
                }

                Debug.Log(successMessage);

                return result;
            }
            catch (Exception e)
            {
                // Display the failure message.
                string failureMessage = $"{method.Method.Name} failed: {e.Message}";
                Debug.LogError(failureMessage);

                throw;
            }
        }
    }
}