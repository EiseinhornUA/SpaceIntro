using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Pause 3 Seconds")]
[UnitCategory("Time")]
public class WaitNode : WaitUnit
{

    protected override IEnumerator Await(Flow flow)
    {
        yield return new WaitForSeconds(3f);
        yield return exit;
    }

}