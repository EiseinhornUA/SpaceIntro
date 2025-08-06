using UnityEngine;

public class LightFollow : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float lightOffsetX = 0f;
    [SerializeField] float lightOffsetY = 2f;

    private void Update()
    {
        Follow();
    }

    public void Follow()
    {
        Vector3 newLightPosition = transform.position;
        newLightPosition.x = player.transform.position.x + lightOffsetX;
        newLightPosition.y = player.transform.position.y + lightOffsetY;
        transform.position = newLightPosition;
    }
}
