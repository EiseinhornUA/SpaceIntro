using System;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private GameObject tabContent;
    [SerializeField] private Button tabButton;
    [SerializeField] private GameObject tabHighlighter;

    public void Select()
    {
        tabContent.SetActive(true);
        tabHighlighter.SetActive(true);
    }

    public void Deselect()
    {
        tabContent.SetActive(false);
        tabHighlighter.SetActive(false);
    }

    internal void AddListener(Action action)
    {
        tabButton.onClick.AddListener(() => action.Invoke());
    }
}