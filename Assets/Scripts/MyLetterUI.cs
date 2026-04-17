using UnityEngine;

public class MyLetterUI : MonoBehaviour
{
    public GameObject letterPrefab;

    public LetterManager letterManager;

    public void OnLetterUIButtonClicked()
    {
        letterManager.SpawnLetter(letterPrefab);
    }
}
