using UnityEngine.InputSystem;
using UnityEngine;

public class BoothInteraction : MonoBehaviour
{
    public SO_Item_Setting Item_Setting;
    protected bool canInteract;
    protected bool isUsed;
    private string playerTag = "Player";

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer SellerRenderer;
    [SerializeField] private Sprite Seller;
    [SerializeField] private Sprite SellerAfterInteract;

    

    protected InventorySystem inventorySystem;

    private void Awake()
    {
        SellerRenderer.sprite = Seller;   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            canInteract = true;
            inventorySystem = collision.GetComponent<InventorySystem>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            canInteract = false;
            inventorySystem = null;
        }
    }

    protected virtual void Update()
    {
        if (!canInteract) return;
        if (isUsed) return;

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            if (inventorySystem != null && inventorySystem.CanAddItem(Item_Setting))
            {
                isUsed = true;
                ChangeSprite();
            }
        }
    }

    protected void ChangeSprite() {
        SellerRenderer.sprite = SellerAfterInteract;
    }
}