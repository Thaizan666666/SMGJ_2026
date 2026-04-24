using System.Collections;
using UnityEngine;
using static ManagerSound;

public class playBGM : MonoBehaviour
{
    public string BGMID = "";
    public bool ShoulPlay = false;

    private void Start()
    {
        StopBGM();
        StopAllLoopEffect();

        if (ShoulPlay)
            StartCoroutine(DelayPlayBGM(BGMID));
    }


    IEnumerator DelayPlayBGM(string bmgID) {

        yield return new WaitForSeconds(2f);
        PlayBGM(BGMID, 1f);
    }
}
