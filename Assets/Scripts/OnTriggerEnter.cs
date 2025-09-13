using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class OnTriggerEnter : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter = new();
    [SerializeField] private int triggerDelay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            OnTriggerActivated().Forget();
        }
    }

    private async UniTask OnTriggerActivated()
    {
        await UniTask.Delay(triggerDelay);
        onTriggerEnter.Invoke();
        gameObject.SetActive(false);
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}
