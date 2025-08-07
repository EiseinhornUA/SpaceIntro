//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Threading.Tasks; // Important: Add this namespace
//using Unity.VisualScripting;
//using UnityEngine;

//// Define the category where your node will appear in the Fuzzy Finder
//[UnitCategory("Utilities/Reflection")]

//// Define the title of your node
//[UnitTitle("Call Async Method")]
//public class CallAMethodAsync : Unit
//{
//    // The flow input port, which triggers the node's execution.
//    [DoNotSerialize]
//    public ControlInput inputTrigger;

//    // The flow output port, which continues the flow after the method is called.
//    [DoNotSerialize]
//    public ControlOutput outputTrigger;

//    // The data input port for the MonoBehaviour object to call the method on.
//    [DoNotSerialize]
//    public ValueInput targetObject;

//    // The data input port for the name of the method to call.
//    [DoNotSerialize]
//    public ValueInput methodName;

//    protected override void Definition()
//    {
//        // Define the input and output ports.
//        inputTrigger = ControlInput("inputTrigger", Trigger);
//        outputTrigger = ControlOutput("outputTrigger");

//        // The 'targetObject' port takes a MonoBehaviour.
//        targetObject = ValueInput<MonoBehaviour>("targetObject", null);

//        // The 'methodName' port takes a string.
//        methodName = ValueInput<string>("methodName", null);

//        // We no longer use a simple succession.
//    }

//    // The async keyword is used here, but the return type remains ControlOutput.
//    // The compiler handles the async state machine logic internally.
//    private async ControlOutput Trigger(Flow flow)
//    {
//        // Get the values from the input ports.
//        MonoBehaviour target = flow.GetValue<MonoBehaviour>(targetObject);
//        string methodToCall = flow.GetValue<string>(methodName);

//        // Basic error checking.
//        if (target == null)
//        {
//            Debug.LogError("Call Async Method Node: Target object is null.");
//            return outputTrigger;
//        }

//        if (string.IsNullOrEmpty(methodToCall))
//        {
//            Debug.LogError("Call Async Method Node: Method name is null or empty.");
//            return outputTrigger;
//        }

//        // Use reflection to find the method.
//        MethodInfo methodInfo = target.GetType().GetMethod(methodToCall, BindingFlags.Public | BindingFlags.Instance);

//        if (methodInfo != null)
//        {
//            try
//            {
//                // Invoke the method and await its completion.
//                // The method must return a `Task` or a similar awaitable type.
//                var task = (Task)methodInfo.Invoke(target, null);
//                if (task != null)
//                {
//                    await task;
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.LogError($"Call Async Method Node: Error invoking method '{methodToCall}' on '{target.name}'. Exception: {e}");
//            }
//        }
//        else
//        {
//            Debug.LogError($"Call Async Method Node: Method '{methodToCall}' not found on '{target.GetType()}'.");
//        }

//        // The flow continues here after the awaited task is complete.
//        return outputTrigger;
//    }
//}