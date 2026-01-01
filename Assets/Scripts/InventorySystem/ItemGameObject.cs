using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;

    public ItemSO GetSO() => itemSO;
}
