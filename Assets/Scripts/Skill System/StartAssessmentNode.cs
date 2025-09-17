using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Start Assessment")]
[UnitCategory("Assessment")]
public class StartAssessmentNode : Unit
{
    [DoNotSerialize] public ControlInput enter;
    [DoNotSerialize] public ControlOutput exit;

    protected override void Definition()
    {
        enter = ControlInput("enter", OnEnter);
        exit = ControlOutput("exit");

        Succession(enter, exit);
    }

    private ControlOutput OnEnter(Flow flow)
    {
        var skillContainer = Object.FindObjectOfType<SkillContainer>(true);
        var skillCapturer = Object.FindObjectOfType<SkillCapturer>(true);

        skillCapturer.CaptureSkills(skillContainer.GetSkills());

        return exit;
    }
}
