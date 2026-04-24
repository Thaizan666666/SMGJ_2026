using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SetUpMAT : MonoBehaviour
{
    public Material sunMaterial;
    
    CharacterStats characterStats;

    private void Start()
    {
        characterStats = FindObjectOfType<CharacterStats>();
        sunMaterial.SetFloat("_SunSlider", 0f);
    }
    private void Update()
    {
        

        if (characterStats == null || sunMaterial == null) return;
        if ((characterStats.currentHotGauge <= characterStats.maxHeatGauge))
        {
            float normalized = Mathf.Clamp01(characterStats.currentHotGauge / 70f);
            sunMaterial.SetFloat("_SunSlider", normalized);
        }
    }

    public void ClearScreen() {
        Debug.LogWarning("Reset _SunSlider");
        sunMaterial.SetFloat("_SunSlider", 0f);
        Debug.LogWarning($"{sunMaterial.GetFloat("_SunSlider")} : SunSlider Pointing");
    }
}
