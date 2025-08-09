using UnityEngine;

public class SpaceSuit : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Mesh suitMesh;
    private Transform characterModelGameObject;
    private SkinnedMeshRenderer characterMesh;

    private void Start()
    {
        
    }

    [ContextMenu("Set Space Suit Mesh")]
    public void SetSpaceSuitMesh()
    {
        characterModelGameObject = player.GetModelTransform();
        characterMesh = characterModelGameObject.GetComponentInChildren<SkinnedMeshRenderer>(includeInactive: false);
        characterMesh.sharedMesh = suitMesh;
    }

}
