using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    [SerializeField] private CharacterContainer characterContainer;
    private GameObject character;

    private void Start()
    {
        character = Instantiate(characterContainer.GetCharacter(PlayerPrefs.GetInt("SelectedCharacter")), parentObject);
    }

    public Animator GetAnimator() => character.GetComponent<Animator>();
}
