using UnityEngine;

[CreateAssetMenu(fileName = "SO_Item", menuName = "SMGJ/SO_Item")]
public class SO_Item_Setting : ScriptableObject
{
    public Sprite ItemSprite;
    public TimedEffectData TimeEffectData;
    public InstantEffectData InstantEffectData;
}
