using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    [SerializeField] ItemContainer itemContainer;
    [SerializeField] ItemSO inventoryItem;
    [SerializeField] GameObject roots;

    public void RemoveRoots()
    {
        if (itemContainer.HasItem(inventoryItem.name))
        {
            roots.SetActive(false);
        }
    }

}
