using UnityEngine;

public class TriggerEventCrab : MonoBehaviour
{
    private MinigameCrab minigame;        // ลาก MinigameCrab มาใส่
    public GameObject minigamePanel;     // ลาก Panel UI มาใส่
    private bool _isTrigger = false;
    private void Awake()
    {
        minigame = FindFirstObjectByType<MinigameCrab>();   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isTrigger)
        {
            minigamePanel.SetActive(true);
            minigame.StartMinigame();
            _isTrigger = true;
        }
    }
}
