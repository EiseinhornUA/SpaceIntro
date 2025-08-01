using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

// Define the category where your node will appear in the Fuzzy Finder
[UnitCategory("Utilities/Reflection")]

// Define the title of your node
[UnitTitle("Call Method")]
public class CallAMethod : Unit
{
    // The flow input port, which triggers the node's execution.
    [DoNotSerialize] // Not necessary to serialize this port
    public ControlInput inputTrigger;

    // The flow output port, which continues the flow after the method is called.
    [DoNotSerialize]
    public ControlOutput outputTrigger;

    // The data input port for the MonoBehaviour object to call the method on.
    [DoNotSerialize]
    public ValueInput targetObject;

    // The data input port for the name of the method to call.
    [DoNotSerialize]
    public ValueInput methodName;

    protected override void Definition()
    {
        // Define the input and output ports.
        inputTrigger = ControlInput("inputTrigger", Trigger);
        outputTrigger = ControlOutput("outputTrigger");

        // The 'targetObject' port takes a MonoBehaviour.
        // The 'false' parameter means it's not a required connection.
        targetObject = ValueInput<MonoBehaviour>("targetObject", null);

        // The 'methodName' port takes a string.
        // It's a required connection, hence the default value is null.
        methodName = ValueInput<string>("methodName", null);

        // A single input port can have multiple outputs, allowing us to connect
        // the flow from the input to the output.
        // In this case, the 'inputTrigger' will be connected to the 'outputTrigger'.
        // The 'Trigger' method is where the magic happens.
        Succession(inputTrigger, outputTrigger);
    }

    private ControlOutput Trigger(Flow flow)
    {
        // Get the values from the input ports.
        MonoBehaviour target = flow.GetValue<MonoBehaviour>(targetObject);
        string methodToCall = flow.GetValue<string>(methodName);

        // Basic error checking.
        if (target == null)
        {
            Debug.LogError("Call Method Node: Target object is null.");
            return outputTrigger;
        }

        if (string.IsNullOrEmpty(methodToCall))
        {
            Debug.LogError("Call Method Node: Method name is null or empty.");
            return outputTrigger;
        }

        // Use reflection to find the method.
        // The BindingFlags.Public flag ensures we only find public methods.
        // The BindingFlags.Instance flag ensures we are looking for methods on an object instance.
        MethodInfo methodInfo = target.GetType().GetMethod(methodToCall, BindingFlags.Public | BindingFlags.Instance);

        if (methodInfo != null)
        {
            // Call the method.
            // The 'null' in Invoke is for parameters. 
            // This example doesn't handle method parameters.
            try
            {
                methodInfo.Invoke(target, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Call Method Node: Error invoking method '{methodToCall}' on '{target.name}'. Exception: {e}");
            }
        }
        else
        {
            Debug.LogError($"Call Method Node: Method '{methodToCall}' not found on '{target.GetType()}'.");
        }

        // Continue the flow to the next node.
        return outputTrigger;
    }
}