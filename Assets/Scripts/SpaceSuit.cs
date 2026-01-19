using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceSuit : MonoBehaviour
{
    [SerializeField] private float particlePositionOffsetX = 0.4f;
    [SerializeField] private float particlePositionOffsetZ = -0.04f;
    private const float particleRotationZ = 90f;
    private const float playerMaxRunningSpeed = 5.5f;
    [SerializeField] private Player player;
    [SerializeField] private Transform playerElbowL;
    [SerializeField] private Transform playerElbowR;
    [SerializeField] private CharacterContainer characterContainer;
    [SerializeField] private Mesh suitMesh;
    [SerializeField] private GameObject helmet;
    [SerializeField] private Material suitMaterial;

    [SerializeField] private GameObject spaceSuitTrailPrefab;
    [SerializeField] private GameObject suitTrailLGameObject;
    [SerializeField] private GameObject suitTrailRGameObject;
    private ParticleSystem suitTrailL;
    private ParticleSystem suitTrailR;

    private bool playerHasSuit = false;

    [ContextMenu("Set Space Suit Mesh")]
    public void PutSpaceSuitOn()
    {
        player.isSpaceSuited = true;
        player.StartFlyingAnimation();
        Transform characterModelGameObject = player.GetModelTransform();
        SkinnedMeshRenderer characterMesh = characterModelGameObject.GetComponentInChildren<SkinnedMeshRenderer>(false);
        characterMesh.sharedMesh = suitMesh;

        CharacterModel characterModel = FindObjectOfType<CharacterModel>();
        if (characterModel)
            Instantiate(helmet, characterModel.head);

        characterMesh.materials = new Material[] { suitMaterial };

        playerHasSuit = true;

        GameObject spaceSuitMesh = gameObject.transform.GetChild(0).gameObject;
        spaceSuitMesh.SetActive(false);
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }

        return null;
    }

    private async void Awake()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        playerElbowL = FindChildRecursive(player.transform, "Elbow_L");
        playerElbowR = FindChildRecursive(player.transform, "Elbow_R");

        suitTrailLGameObject = Instantiate(
            spaceSuitTrailPrefab,
            new Vector3(playerElbowL.position.x - particlePositionOffsetX, 
                playerElbowL.position.y, playerElbowL.position.z - particlePositionOffsetZ),
            playerElbowL.localRotation * Quaternion.Euler(0f, 0f, -particleRotationZ),
            playerElbowL
        );
        suitTrailRGameObject = Instantiate(
            spaceSuitTrailPrefab,
            new Vector3(playerElbowR.position.x + particlePositionOffsetX, 
                playerElbowR.position.y, playerElbowR.position.z - particlePositionOffsetZ),
            playerElbowR.localRotation * Quaternion.Euler(0f, 0f, particleRotationZ),
            playerElbowR
        );

        suitTrailL = suitTrailLGameObject.GetComponent<ParticleSystem>();
        suitTrailR = suitTrailRGameObject.GetComponent<ParticleSystem>();
        suitTrailLGameObject.GetComponent<ParticleSystem>().Stop();
        suitTrailRGameObject.GetComponent<ParticleSystem>().Stop();
    }

    private void Update()
    {
        if (!playerHasSuit) return;

        bool particleActivationCondition = Math.Abs(player.velocity.x) > playerMaxRunningSpeed;
        if (particleActivationCondition && !suitTrailL.isPlaying)
        {
            suitTrailL.Play();
            suitTrailR.Play();
        }
        if (!particleActivationCondition && suitTrailL.isPlaying)
        {
            suitTrailL.Stop();
            suitTrailR.Stop();
        }
    }

    public bool HasPlayerSuit() => playerHasSuit;
}
