using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Glass : MonoBehaviour
{
    [SerializeField] private float openDuration = 1f;

    [ContextMenu("Open Glass")]
    public void RemoveGlass()
    {
        gameObject.transform.DOScaleY(0f, openDuration);
    }
}
