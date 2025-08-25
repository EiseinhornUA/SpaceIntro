using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private List<Tab> tabs;
    private int previousTabIndex;
    [SerializeField] private int initialTabIndex = 0;

    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int capturedIndex = i;
            tabs[i].AddListener(() => SwitchTab(capturedIndex));
            tabs[i].Deselect();
        }
        tabs[initialTabIndex].Select();
    }

    public void SwitchTab(int index)
    {
        tabs[previousTabIndex].Deselect();
        tabs[index].Select();
        previousTabIndex = index;
    }
}
