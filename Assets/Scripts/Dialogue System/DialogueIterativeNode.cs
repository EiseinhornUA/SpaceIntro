using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using Cysharp.Threading.Tasks;

[UnitTitle("Dialogue Iterative Node")]
[UnitCategory("Dialogue")]
public class DialogueIterativeNode : WaitUnit
{
    [Inspectable]
    [UnitHeaderInspectable("Choice Count")]
    [Range(2, 4)]
    public int choiceCount = 2;

    private ValueInput messageInput;
    private ValueInput characterInput;
    private ValueInput respondentInput;

    private List<ValueInput> choiceInputs = new();
    private List<ValueInput> skillInputs = new();

    private DialogueView view;
    private new ControlInput enter;

    private SkillSO skill;

    protected override void Definition()
    {
        choiceInputs.Clear();

        messageInput = ValueInput<string>("Dialogue Line", "");
        characterInput = ValueInput<DialogueCharacter>("Character", null);
        respondentInput = ValueInput<DialogueCharacter>("Respondent", null);
        enter = ControlInputCoroutine("enter", Await);

        var exit = ControlOutput("");

        for (int i = 0; i < choiceCount; i++)
        {
            choiceInputs.Add(ValueInput<string>($"Text{i + 1}", ""));
            skillInputs.Add(ValueInput<SkillSO>($"Skill {i + 1}", null));

            Succession(enter, exit);
        }
    }

    protected override IEnumerator Await(Flow flow)
    {
        SkillContainer skillContainer = GameObject.FindObjectOfType<SkillContainer>(includeInactive: true);
        view = GameObject.FindObjectOfType<DialogueView>();

        var message = flow.GetValue<string>(messageInput);
        var requester = flow.GetValue<DialogueCharacter>(characterInput);

        if (!requester)
        {
            Debug.LogError(requester + " is null. Please assign a character to the Dialogue Choice Node.");
            yield break;
        }

        SetupDialogueView(message, requester);

        var skills = skillInputs.Select(input => flow.GetValue<SkillSO>(input)).ToList();
        var choices = choiceInputs.Select(input => flow.GetValue<string>(input)).ToList();

        int selectedIndex = -1;

        for (int i = 0; i < choiceInputs.Count - 1; i++)
        {
            view.ShowChoices(choices);
            yield return view.WaitForChoice().ContinueWith(i => selectedIndex = i).ToCoroutine();

            float skillPoints = CalculateSkillPoints(i);
            skill = skills[selectedIndex];

            if (skill != null)
            {
                skillContainer.AddSkillLevel(skill, skillPoints);
                //Debug.Log($"Added {amount} points to {skill.name}");
            }

            choices.Remove(choices[selectedIndex]);
            skills.Remove(skills[selectedIndex]);
        }

        var respondent = flow.GetValue<DialogueCharacter>(respondentInput);

        SetupDialogueView(message: choices[0], respondent);

        yield return view.WaitForClick().ToCoroutine();
        yield return exit;
    }

    private void SetupDialogueView(string message, DialogueCharacter requester)
    {
        view.SetCharacterName(requester.GetName());
        view.ChangeCharacterPortrait(requester.GetPortrait());
        view.SetMessage(message);
    }

    private int CalculateSkillPoints(int i) => choiceInputs.Count - 1 - i;
}
