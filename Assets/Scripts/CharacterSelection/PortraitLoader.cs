using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class PortraitLoader : MonoBehaviour
{
    [SerializeField] private DialogueCharacter character;

    private void Start()
    {
        character.SetPortrait(PlayerPrefs.GetString("PortraitPath", string.Empty));
        Debug.Log("Portrait loaded from: " + PlayerPrefs.GetString("PortraitPath", "No path set"));
    }
}
