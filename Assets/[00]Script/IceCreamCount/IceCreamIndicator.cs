using UnityEngine;
using UnityEngine.UI;

public class IceCreamIndicator : MonoBehaviour
{
    private IceCreamCount creamCount;
    private Slider IceCreamSlider;
    public Sprite emptySprite;

    private void Awake()
    {
        creamCount = FindFirstObjectByType<IceCreamCount>();
        IceCreamSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        IceCreamSlider.value = creamCount.CurrentTime / creamCount.totalTime;
    }


}
