using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private Button interactionButton;
}
