using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static ManagerSound;

public class MOM : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject BTN_E;
    private SetUpMAT stm;

    private IceCreamCount GotIceCream;

    private void Start()
    {
        GotIceCream = FindFirstObjectByType<IceCreamCount>();
        BTN_E?.SetActive(false);
        stm = FindFirstObjectByType<SetUpMAT>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Only show BTN_E if the player is actually carrying ice cream
        if (GotIceCream != null && GotIceCream.HasStarted && !GotIceCream.IsFinished)
            BTN_E?.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        BTN_E?.SetActive(false);
    }

    private void Update()
    {
        if (GotIceCream == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StopAllLoopEffect();
            if (GotIceCream.HasStarted)
            {
                BTN_E?.SetActive(false);

                if (!GotIceCream.IsFinished)
                {
                    PlayEffect("GoodEnding");
                    stm.ClearScreen();
                    ManagerScene.Instance.LoadHappyEnding();
                }
                else
                {
                    Debug.Log("---------------------------------------Start Ending");
                    stm.ClearScreen();
                    PlayEffect("BadEnding");
                    ManagerScene.Instance.LoadMomSadEnding();
                }
            }
        }
    }
}