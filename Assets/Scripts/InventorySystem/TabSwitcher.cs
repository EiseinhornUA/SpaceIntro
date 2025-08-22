using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private List<TabButtonPair> tabButtonPairs;
    private int previousTabIndex;

    private void Start()
    {
        for (int i = 0; i < tabButtonPairs.Count; i++)
        {
            int capturedIndex = i;
            tabButtonPairs[i].button.onClick.AddListener(() => SwitchTab(capturedIndex));
        }
    }

    public void SwitchTab(int index)
    {
        tabButtonPairs[index].tab.SetActive(true);
        tabButtonPairs[previousTabIndex].tab.SetActive(false);
        previousTabIndex = index;
    }
}

[System.Serializable]
public class TabButtonPair
{
    public GameObject tab;
    public Button button;
}
