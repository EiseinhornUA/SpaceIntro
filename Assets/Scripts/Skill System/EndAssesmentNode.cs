using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Save Assessment")]
[UnitCategory("Assessment")]
public class EndAssessmentNode : Unit
{
    [DoNotSerialize] public ControlInput enter;
    [DoNotSerialize] public ControlOutput exit;

    [DoNotSerialize] public ValueInput assessmentNameInput;

    protected override void Definition()
    {
        enter = ControlInput("enter", OnEnter);
        exit = ControlOutput("exit");

        assessmentNameInput = ValueInput<string>("assessment name", default);

        Requirement(assessmentNameInput, enter);
        Succession(enter, exit);
    }

    private ControlOutput OnEnter(Flow flow)
    {
        var skillContainer = Object.FindObjectOfType<SkillContainer>(true);
        var skillCapturer = Object.FindObjectOfType<SkillCapturer>(true);
        var databaseManager = Object.FindObjectOfType<DatabaseManager>(true);

        string assessmentName = flow.GetValue<string>(assessmentNameInput);

        var skills = skillCapturer.GetSkills();

        var skillsDelta = skillContainer.GetSkills();

        for (int i = 0; i < skillsDelta.Count; i++)
        {
            skillsDelta[i] = new Skill(skillsDelta[i].skillName, skillContainer.GetSkills()[i].level - skills[i].level);
        }

        databaseManager.SaveAssesment(skillsDelta, assessmentName);

        return exit;
    }
}
