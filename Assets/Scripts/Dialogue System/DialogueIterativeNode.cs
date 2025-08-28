using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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
    //private List<ControlOutput> exits = new();

    private DialogueView view;
    private new ControlInput enter;

    private UniTask<int> task;
    private SkillSO skill;

    protected override void Definition()
    {
        choiceInputs.Clear();
        //exits.Clear();

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
        var character = flow.GetValue<DialogueCharacter>(characterInput);

        if (!character)
        {
            Debug.LogError(character + " is null. Please assign a character to the Dialogue Choice Node.");
            yield break;
        }

        view.SetCharacterName(character.GetName());
        view.ChangeCharacterPortrait(character.GetPortrait());
        view.SetMessage(message);


        var choices = new List<string>();
        var skills = new List<SkillSO>();

        for (int i = 0; i < choiceCount; i++)
        {
            choices.Add(flow.GetValue<string>(choiceInputs[i]));
            skills.Add(flow.GetValue<SkillSO>(skillInputs[i]));
        }

        int selectedIndex = -1; // ?????

        for (int i = 0; i < choiceInputs.Count - 1; i++)
        {
            view.ShowChoices(choices);
            task = view.WaitForChoice();
            yield return task.ContinueWith(i => selectedIndex = i).ToCoroutine();

            float amount = 3 - i;
            skill = skills[selectedIndex];

            if (skill != null)
            {
                skillContainer.AddSkillLevel(skill, amount);
                //Debug.Log($"Added {amount} points to {skill.name}");
            }

            choices.Remove(choices[selectedIndex]);
            skills.Remove(skills[selectedIndex]);
        }

        var respondent = flow.GetValue<DialogueCharacter>(respondentInput);

        view.SetCharacterName(respondent.GetName());
        view.ChangeCharacterPortrait(respondent.GetPortrait());
        view.SetMessage(choices[0]);

        yield return view.WaitForClick().ToCoroutine();
        yield return exit;
    }
}
