using UnityEngine;

[ExecuteAlways]
public class ParentFitChild : MonoBehaviour
{
    [SerializeField] RectTransform child;
    [SerializeField] Vector2 sizeOffset = Vector2.zero;
    [SerializeField] bool everyFrame = true;

    RectTransform parent;

    void Awake()
    {
        parent = transform as RectTransform;
        Apply();
    }

    void Update()
    {
        if (everyFrame) Apply();
    }

    void OnValidate()
    {
        Apply();
    }

    void Apply()
    {
        if (!parent || !child) return;

        parent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            child.rect.width + sizeOffset.x
        );

        parent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            child.rect.height + sizeOffset.y
        );
    }
}

