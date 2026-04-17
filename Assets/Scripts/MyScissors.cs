using UnityEngine;

public class MyScissors : MonoBehaviour
{
    public LetterController currentLetter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter: " + other.name + " at " + Time.time);

        if (currentLetter == null) return;

        if (other.gameObject == currentLetter.gameObject)
        {
            Debug.Log("StartCutting triggered by: " + other.name);
            currentLetter.StartCutting();
        }
    }

}
