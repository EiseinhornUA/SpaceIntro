using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;

    [ContextMenu("Show Fade Out")]
    public void ShowFadeOut()
    {
        fadeImage.DOFade(1f, fadeSpeed);
    }

    [ContextMenu("Show Fade In")]
    public void ShowFadeIn()
    {
        fadeImage.DOFade(0f, fadeSpeed);
    }
}
