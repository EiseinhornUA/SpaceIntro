using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject playerLight;
    [SerializeField] private GameObject globalLight;

    public bool isGlobalLightFixed() => globalLight.activeSelf;

    public void SwitchToPlayerLight()
    {
        playerLight.SetActive(true);
        globalLight.SetActive(false);
    }

    public void SwitchToGlobalLight()
    {
        playerLight.SetActive(false);
        globalLight.SetActive(true);
    }
}

