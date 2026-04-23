using System.Collections;
using UnityEngine;

public class TriggerEventCrab : MonoBehaviour
{
    private MinigameCrab minigame;        // ลาก MinigameCrab มาใส่
    private bool _isTrigger = false;
    [Header("Setting")]
    public float coolDown = 3f;
    private float count = 0f;

    private void Awake()
    {
        minigame = FindFirstObjectByType<MinigameCrab>();   
    }
    private void Update()
    {
        if(minigame.isFinish == true)
        {
            StartCoroutine(OnCoolDown());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isTrigger)
        {
            minigame.StartMinigame();
            _isTrigger = true;
            ManagerSound.PlayEffect("Pain");
            ManagerSound.PlayEffect("Crab");
            Debug.Log("-------------------------------------------------------------------------");
        }
    }
    IEnumerator OnCoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        _isTrigger = false;
    }
}
