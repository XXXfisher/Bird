using UnityEngine;

public class PaperStateController : MonoBehaviour
{
    [Header("Paper State")]
    public GameObject state0_full;
    public GameObject state1_ICut;
    public GameObject state2_YouCut;
    public GameObject state3_AllCut;

    private bool isICut = false;
    private bool isYouCut = false;

    public void OnClickITrigger()
    {
        if (isICut) return;
        isICut = true;
        UpdatePaperDisplay();
    }

    public void OnClickYouTrigger()
    {
        if (isYouCut) return;
        isYouCut = true;
        UpdatePaperDisplay();
    }

    private void UpdatePaperDisplay()
    {
        state0_full.SetActive(false);
        state1_ICut.SetActive(false);
        state2_YouCut.SetActive(false);
        state3_AllCut.SetActive(false);

        if (isICut && isYouCut)
        {
            state3_AllCut.SetActive(true);
        }
        else if (isICut)
        {
            state1_ICut.SetActive(true);
        }
        else if (isYouCut)
        {
            state2_YouCut.SetActive(true);
        }
        else
        {
            state0_full.SetActive(true);
        }
    }
}
