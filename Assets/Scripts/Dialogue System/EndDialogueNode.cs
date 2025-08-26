using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

[UnitTitle("End Dialogue Node")]
[UnitCategory("Dialogue")]
public class EndDialogueNode : Unit
{
    private ControlOutput exit;
    private List<ControlInput> enters = new();
    private List<ValueInput> variantNames = new();

    [UnitHeaderInspectable("Variant Count")]
    [Range(1, 16)]
    public int exitCount = 1;
    private List<Decision> decisions;

    protected override void Definition()
    {
        exit = ControlOutput("exit");

        //FindObjectOfType not allowed in Definition
        for (int i = 0; i < exitCount; i++)
        {
            var nameInput = ValueInput<string>($"Variant Name {i + 1}", " ");
            variantNames.Add(nameInput);

            int capturedIndex = i; // Capture index properly
            var enter = ControlInput($"Dialogue {i + 1} Exit", flow => OnEnter(flow, capturedIndex));
            enters.Add(enter);

            Succession(enter, exit);
        }

    }


    private ControlOutput OnEnter(Flow flow, int index)
    {
        var dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
        if (dialogueManager)
        {
            string decisionName = "";
            if (variantNames.Count == 0)
                decisionName = flow.GetValue<string>(variantNames[index]);
            else
                decisionName = " ";
            dialogueManager.SetSelectedDecision(new Decision(decisionName, index));
        }

        GameObject.FindObjectOfType<DialogueView>().gameObject.SetActive(false);
        return exit;
    }

}
