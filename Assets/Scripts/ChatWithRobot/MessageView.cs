using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class MessageView : MonoBehaviour
{
    //[SerializeField] private string characterName;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI messageText;
    public static MessageView Create(PhraseCharacterPair message, MessageView prefab, Transform transform)
    {
        var instance = Instantiate(prefab, transform);

        instance.messageText.text = message.phrase;
        //instance.characterName.text = message.character.GetName();
        instance.characterPortrait.sprite = message.character.GetPortrait();

        return instance;
    }

    private void OnEnable()
    {
        UpdateRectTransform().Forget();
    }

    private async UniTask UpdateRectTransform()
    {
        await UniTask.WaitForEndOfFrame(this);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}