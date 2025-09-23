using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    [SerializeField] private CharacterContainer characterContainer;
    private Animator animator;

    private GameObject character;

    public event Action<List<Skill>> OnCharacterLoaded = delegate { };

    private void Start()
    {
        ClearChildren();

        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        character = Instantiate(characterContainer.GetCharacter(index), parentObject);

        OnCharacterLoaded?.Invoke(characterContainer.GetInitialSkills(index));

        animator = character.GetComponent<Animator>();
    }

    private void ClearChildren()
    {
        foreach (Transform child in parentObject)
            Destroy(child.gameObject);
    }

    public Animator GetAnimator() => character ? animator : null;
}
