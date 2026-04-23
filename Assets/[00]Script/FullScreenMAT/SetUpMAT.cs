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
    }
    private void Update()
    {
        if (characterStats == null || sunMaterial == null) return;

        float normalized = Mathf.Clamp01(characterStats.currentHotGauge / 70f);
        sunMaterial.SetFloat("_SunSlider", normalized);
    }
}
