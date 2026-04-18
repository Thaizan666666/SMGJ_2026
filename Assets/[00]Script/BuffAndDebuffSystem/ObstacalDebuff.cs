using UnityEngine;

public class ObstacalDebuff : MonoBehaviour
{
    [SerializeField] TimedEffectData slowData;
    [SerializeField] TimedEffectData heatSurgeData;
    [SerializeField] InstantEffectData heatBlastData;

    void OnTriggerEnter2D(Collider2D other)
    {
        var em = other.GetComponent<EffectManager>();
        
        if (em == null) return;
        Debug.LogWarning("have ");

        // Timed — refreshes if already active
        if(slowData != null)
        em.ApplyTimed(slowData);

        if(heatSurgeData != null)
        em.ApplyTimed(heatSurgeData);

        // Instant — fires once, permanent
        if(heatBlastData != null)
        em.ApplyInstant(heatBlastData);
    }
}
