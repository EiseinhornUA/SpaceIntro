using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 position)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
    }
}
