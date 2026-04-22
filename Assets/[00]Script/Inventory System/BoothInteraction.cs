using UnityEngine.InputSystem;
using UnityEngine;

public class BoothInteraction : MonoBehaviour
{
    public SO_Item_Setting Item_Setting;
    private bool canInteract;
    private bool isUsed;
    private string playerTag = "Player";

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer SellerRenderer;
    [SerializeField] private Sprite Seller;
    [SerializeField] private Sprite SellerAfterInteract;

    

    private InventorySystem inventorySystem;

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

    private void Update()
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

    private void ChangeSprite() {
        SellerRenderer.sprite = SellerAfterInteract;
    }
}