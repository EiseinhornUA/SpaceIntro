using System;
using System.Collections;
using UnityEngine;

public class IndicatorSwitcher : MonoBehaviour
{
    [Header("Indicators")]
    [SerializeField] private GameObject inventoryIndicator;
    [SerializeField] private GameObject tasksIndicator;
    [SerializeField] private GameObject reportsIndicator;
    [SerializeField] private GameObject backpackIndicator;
    [SerializeField] private GameObject chatIndicator;

    [Header("Tabs")]
    [SerializeField] private Tab tasksTab;
    [SerializeField] private Tab reportsTab;
    [SerializeField] private Tab backpackTab;
    [SerializeField] private Tab chatTab;

    [Header("InventoryViews")]
    [SerializeField] private JournalView journalView;
    [SerializeField] private ReportContainerView reportsView;
    [SerializeField] private BackpackView backpackView;
    [SerializeField] private RobotChatHistoryView chatView;


    private void Awake()
    {
        tasksTab.AddListener(() => DisableIndicator(tasksIndicator));
        reportsTab.AddListener(() => DisableIndicator(reportsIndicator));
        backpackTab.AddListener(() => DisableIndicator(backpackIndicator));
        chatTab.AddListener(() => DisableIndicator(chatIndicator));


        journalView.onTaskAdded += () => EnableIndicator(tasksIndicator);
        reportsView.onReportAdded += () => EnableIndicator(reportsIndicator);
        backpackView.onItemAdded += () => EnableIndicator(backpackIndicator);
        chatView.onMessageAdded += () => EnableIndicator(chatIndicator);
    }

    private void Start()
    {
        DisableIndicator(inventoryIndicator);
        DisableIndicator(tasksIndicator);
        DisableIndicator(reportsIndicator);
        DisableIndicator(backpackIndicator);
        DisableIndicator(chatIndicator);
    }

    private void DisableIndicator(GameObject indicator)
    {
        indicator.SetActive(false);

        if (!tasksIndicator.activeSelf && !reportsIndicator.activeSelf && !backpackIndicator.activeSelf && !chatIndicator.activeSelf)
        {
            inventoryIndicator.SetActive(false);
        }
    }

    private void EnableIndicator(GameObject indicator)
    {
        indicator.SetActive(true);
        inventoryIndicator.SetActive(true);
    }
}