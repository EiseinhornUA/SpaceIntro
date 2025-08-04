using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;

    private bool enableFadeOut = true;

    void Update()
    {
        Color color = fadeImage.color;
        float alphaChange = fadeSpeed * Time.deltaTime;

        if (enableFadeOut && color.a > 0f)
        {
            color.a -= alphaChange;
            color.a = Mathf.Clamp01(color.a);
            fadeImage.color = color;
        }
        else if (!enableFadeOut && color.a < 1f)
        {
            color.a += alphaChange;
            color.a = Mathf.Clamp01(color.a);
            fadeImage.color = color;
        }
    }

    public void EnableFadeOut()
    {
        enableFadeOut = true;
    }

    public void EnableFadeIn()
    {
        enableFadeOut = false;
    }

    [ContextMenu("Show Fade Out")]
    public void ShowFadeOut()
    {
        fadeImage.color = new Color(1f, 1f, 1f, 1f);
        EnableFadeOut();
    }

    [ContextMenu("Show Fade In")]
    public void ShowFadeIn()
    {
        fadeImage.color = new Color(1f, 1f, 1f, 0f);
        EnableFadeIn();
    }
}
