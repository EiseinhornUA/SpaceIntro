using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintNumber : MonoBehaviour
{
    [SerializeField] private GameObject numPad;
    [SerializeField] private string printSymbol;
    [SerializeField] private TMPro.TextMeshProUGUI numPadEntry;
    [SerializeField] private Player player;
    [SerializeField] private GameObject axe;

    public void TypeNumber()
    {
        if (printSymbol == "×")
        {
            if (!string.IsNullOrEmpty(numPadEntry.text))
            {
                numPadEntry.text = numPadEntry.text.Substring(0, numPadEntry.text.Length - 1);
            }
            return;
        }

        if (printSymbol == "#")
        {
            int value;
            if (int.TryParse(numPadEntry.text, out value) && value == 3264)
            {
                axe.transform.position = player.transform.position;
                numPad.SetActive(false);
            }

            numPadEntry.text = "";
            return;
        }

        numPadEntry.text += printSymbol;
    }
}
