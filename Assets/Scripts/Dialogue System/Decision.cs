using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Decision
{
    [SerializeField] private string decisionText;
    [SerializeField] private int decisionIndex;

    public Decision(string title, int index)
    {
        decisionText = title;
        decisionIndex = index;
    }

    public void SetText(string text) => decisionText = text;
    public string GetText() => decisionText;

    public void SetIndex(int index) => decisionIndex = index;
    public int GetIndex() => decisionIndex;
}
