using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCompleter : MonoBehaviour
{
    [ContextMenu("Complete Game")]
    public void CompleteGame()
    {
        SceneManager.LoadScene("Character Seleciton");
        GameStateProvider.SetCompleted();
    }
}
