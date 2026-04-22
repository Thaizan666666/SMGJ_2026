using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LuckyBooth : BoothInteraction
{
    public List<ItemEntry> ItemsList;

    [System.Serializable] // FIX 1: needed to show in Inspector
    public class ItemEntry
    {
        public SO_Item_Setting Item;
        [Range(0f, 100f)] public float Weight = 100f;
    }

    protected override void Update()
    {
        if (!canInteract) return;
        if (isUsed) return;

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            SO_Item_Setting randomItem = GetRandomItem(); // FIX 3: removed stray dot

            if (randomItem != null && inventorySystem != null && inventorySystem.CanAddItem(randomItem)) // FIX 4: pass randomItem
            {
                isUsed = true;
                ChangeSprite();
            }
        }
    }

    private SO_Item_Setting GetRandomItem() // FIX 2: return type was GameObject
    {
        float totalWeight = 0f;
        foreach (var entry in ItemsList)
            totalWeight += entry.Weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in ItemsList)
        {
            cumulative += entry.Weight;
            if (roll < cumulative)
                return entry.Item;
        }

        return null;
    }
}