using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    private EffectManager effectManager;
    private SO_Item_Setting currentItem;
    [SerializeField] private KeyCode UseKey = KeyCode.Q;
    [SerializeField] private Image SlotItem;

    private void Awake()
    {
        effectManager = GetComponent<EffectManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(UseKey))
        {
            UseItem();
        }
    }

    // Adds item only if the slot is empty
    public void AddItemToInventory(SO_Item_Setting incomingItem)
    {
        currentItem = incomingItem;
        UpdateUI();
        Debug.Log($"Picked up: {incomingItem.name}");
    }

    // Returns true if the slot is free (and adds the item)
    public bool CanAddItem(SO_Item_Setting incomingItem)
    {
        if (currentItem == null)   // FIX: was != null (backwards)
        {
            AddItemToInventory(incomingItem);
            return true;
        }
        else
        {
            Debug.Log("Inventory full! Use your current item first (Q).");
            return false;
        }
    }

    private void UseItem()
    {
        if (currentItem == null)
        {
            Debug.Log("No item to use.");
            return;
        }

        Debug.Log($"Using item: {currentItem.name}");

        // Apply whichever effect is set on the item
        if (currentItem.InstantEffectData != null)
            effectManager.ApplyInstant(currentItem.InstantEffectData);

        if (currentItem.TimeEffectData != null)
            effectManager.ApplyTimed(currentItem.TimeEffectData);

        currentItem = null; // Consume the item
        UpdateUI();
    }

    public bool HasItem() => currentItem != null;

    public void UpdateUI()
    {
        SlotItem.sprite = currentItem != null ? currentItem.ItemSprite : null;
    }
}