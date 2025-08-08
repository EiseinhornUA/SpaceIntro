using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Utilities/Reflection")]
[UnitTitle("Call Async Method")]
public class CallAsyncMethod : WaitUnit
{
    [DoNotSerialize]
    public ValueInput targetObject;

    [DoNotSerialize]
    public ValueInput methodName;

    protected override void Definition()
    {
        targetObject = ValueInput<MonoBehaviour>("targetObject", null);

        methodName = ValueInput<string>("methodName", null);
        base.Definition();
    }

    protected override IEnumerator Await(Flow flow)
    {
        MonoBehaviour target = flow.GetValue<MonoBehaviour>(targetObject);
        string methodToCall = flow.GetValue<string>(methodName);

        // Basic error checking.
        if (target == null)
        {
            Debug.LogError("Call Method Node: Target object is null.");
            yield break;
        }

        if (string.IsNullOrEmpty(methodToCall))
        {
            Debug.LogError("Call Method Node: Method name is null or empty.");
            yield break;
        }

        System.Reflection.MethodInfo methodInfo = target.GetType().GetMethod(methodToCall);
        object result = methodInfo.Invoke(target, null);

        // Check if the result is a UniTask
        if (result is UniTask task)
        {
            yield return task.ToCoroutine();
        }
        else
        {
            Debug.LogError("Method does not return a UniTask.");
        }
        yield return exit;
    }
}