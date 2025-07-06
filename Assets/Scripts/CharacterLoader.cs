using UnityEngine;
using UnityEngine.Events;

public class CharacterLoader : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    [SerializeField] private CharacterContainer characterContainer;
    private Animator animator;

    private GameObject character;

    private void Start()
    {
        ClearChildren();

        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        character = Instantiate(characterContainer.GetCharacter(index), parentObject);

        animator = character.GetComponent<Animator>();
    }

    private void ClearChildren()
    {
        foreach (Transform child in parentObject)
            Destroy(child.gameObject);
    }

    public Animator GetAnimator() => character ? animator : null;
}
