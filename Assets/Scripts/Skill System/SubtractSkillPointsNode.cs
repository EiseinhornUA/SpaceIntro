using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[UnitTitle("Subtract Skill Points Node")]
[UnitCategory("Skills")]
public class SubtractSkillPointsNode : Unit
{
    private List<ValueInput> amountInputs = new();
    private List<ValueInput> skillInputs = new();

    private ControlInput enter;
    private ControlOutput exit;

    [UnitHeaderInspectable("Skill Count")]
    [Range(1, 24)]
    public int skillCount = 1;

    protected override void Definition()
    {
        amountInputs.Clear();
        skillInputs.Clear();

        enter = ControlInput("", OnEnter);
        exit = ControlOutput("");

        for (int i = 0; i < skillCount; i++)
        {
            amountInputs.Add(ValueInput<uint>($"Amount {i + 1}", 0));
            skillInputs.Add(ValueInput<SkillSO>($"Skill {i + 1}", null));
        }

        Succession(enter, exit);
    }

    protected ControlOutput OnEnter(Flow flow)
    {
        SkillContainer skillContainer = GameObject.FindObjectOfType<SkillContainer>(includeInactive: true);
        for (int i = 0; i < skillCount; i++)
        {
            int amount = (int)flow.GetValue<uint>(amountInputs[i]);
            SkillSO skill = flow.GetValue<SkillSO>(skillInputs[i]);

            if (skill != null)
            {
                skillContainer.SubtractSkillLevel(skill, amount);
                Debug.Log($"Subtracted {amount} skill points from {skill.GetName()}. Current level: {skill.GetLevel()}");
            }
        }

        return exit;
    }
}
