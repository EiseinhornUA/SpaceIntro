using System;
using UnityEngine;

public class SpaceSuit : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Mesh suitMesh;
    [SerializeField] private GameObject helmet;

    private Transform characterModelGameObject;
    private SkinnedMeshRenderer characterMesh;
    private Transform characterHead;

    private void Start()
    {
        SetSpaceSuitMesh();
    }

    [ContextMenu("Set Space Suit Mesh")]
    public void SetSpaceSuitMesh()
    {
        characterModelGameObject = player.GetModelTransform();
        characterMesh = characterModelGameObject.GetComponentInChildren<SkinnedMeshRenderer>(false);
        characterMesh.sharedMesh = suitMesh;
        characterHead = FindChildRecursive(characterModelGameObject, "Head");
        Instantiate(helmet, characterHead);
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            var result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
