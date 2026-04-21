using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public LetterController lc;
    public int myID; // 左->右：0->1

    private bool isScissorsOver = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scissors")) isScissorsOver = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Scissors")) isScissorsOver = false;
    }

    private void Update()
    {
        // 玩家按住剪刀拖到这里，松手的一瞬间，触发裁剪
        if (isScissorsOver && Input.GetMouseButtonUp(0))
        {
            lc.StartCutting(myID);
            gameObject.SetActive(false); // 触发后自身失效
        }
    }
}




