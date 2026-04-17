using UnityEngine;

[CreateAssetMenu(fileName = "SunSystemSetting", menuName = "SMGJ/SunSystemSetting")]
public class CoverSystemSetting : ScriptableObject
{
    [Header("Target Name")]
    public string PlayerTag = "Player";
    public LayerMask PlayerLayer;

    [Header("Overlap Sampling of Player")]
    [Tooltip("Grid resolution per axis. 6 = 6x6 = 36 sample points. Higher = more accurate but costs more CPU.")]
    [Range(2, 6)] public int SampleResolution = 3;

    [Header("Hot Gauge System Setting")]
    public float HotGaugeIncreaseRate = 5; //per second
    public float HotGaugeDecreaseRate = 3; //per second

    
}
