using System;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private GameObject tabContent;
    [SerializeField] private Button tabButton;
    [SerializeField] private GameObject tabHighlighter;
    [SerializeField] private GameObject tabIndicator;

    private void Start()
    {
        tabIndicator.SetActive(false);
    }

    public void Select()
    {
        tabContent.SetActive(true);
        tabHighlighter.SetActive(true);
        tabIndicator.SetActive(false);
    }

    public void Deselect()
    {
        tabContent.SetActive(false);
        tabHighlighter.SetActive(false);
    }

    public void EnableIndicator()
    {
        tabIndicator.SetActive(true);
    }

    internal void AddListener(Action action)
    {
        tabButton.onClick.AddListener(() => action.Invoke());
    }
}