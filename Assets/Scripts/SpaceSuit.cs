using System;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceSuit : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Mesh suitMesh;
    [SerializeField] private GameObject helmet;
    [SerializeField] private Material suitMaterial;

    [ContextMenu("Set Space Suit Mesh")]
    public void PutSpaceSuitOn()
    {
        Transform characterModelGameObject = player.GetModelTransform();
        SkinnedMeshRenderer characterMesh = characterModelGameObject.GetComponentInChildren<SkinnedMeshRenderer>(false);
        characterMesh.sharedMesh = suitMesh;

        CharacterModel characterModel = FindObjectOfType<CharacterModel>();
        if (characterModel)
            Instantiate(helmet, characterModel.head);

        characterMesh.materials = new Material[] { suitMaterial };

        gameObject.SetActive(false);
    }
}
