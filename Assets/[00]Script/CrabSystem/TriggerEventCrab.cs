using UnityEngine;

public class TriggerEventCrab : MonoBehaviour
{
    public MinigameCrab minigame;        // ลาก MinigameCrab มาใส่
    public GameObject minigamePanel;     // ลาก Panel UI มาใส่
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            minigamePanel.SetActive(true);
            minigame.StartMinigame();
        }
    }
}
