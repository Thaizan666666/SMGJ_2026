using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MOM : MonoBehaviour
{
    public UnityEvent onHappyEnding;
    public UnityEvent onSadEnding;
    private IceCreamCount GotIceCream;

    private void Start()
    {
        GotIceCream = FindFirstObjectByType<IceCreamCount>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GotIceCream.HasStarted == true)
        {
            if(GotIceCream.IsFinished == false)
            {
                ManagerScene.Instance.LoadHappyEnding();
                onHappyEnding?.Invoke();
            }
            else
            {
                onSadEnding?.Invoke();
            }
        }
    }
}
