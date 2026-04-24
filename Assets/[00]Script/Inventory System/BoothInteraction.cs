using UnityEngine.InputSystem;
using UnityEngine;
using static ManagerSound;
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
    [SerializeField] private GameObject BTN_E;
    [SerializeField] private GameObject Cross;

    protected InventorySystem inventorySystem;

    private void Awake()
    {
        SellerRenderer.sprite = Seller;
        BTN_E.SetActive(false);
        Cross.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        canInteract = true;
        inventorySystem = collision.GetComponent<InventorySystem>();

        if (isUsed) return;

        // Show the button immediately on enter, before Update() runs
        BTN_E.SetActive(true);
        Cross.SetActive(inventorySystem != null && inventorySystem.HasItem());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only react to the player leaving — ignore all other colliders
        if (!collision.CompareTag(playerTag)) return;

        canInteract = false;
        inventorySystem = null;
        BTN_E.SetActive(false);
        Cross.SetActive(false);
    }

    protected virtual void Update()
    {
        if (!canInteract) return;
        if (isUsed) return;

        // Keep cross in sync with inventory state every frame
        Cross.SetActive(inventorySystem != null && inventorySystem.HasItem());

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            if (inventorySystem != null && inventorySystem.CanAddItem(Item_Setting))
            {
                PlayEffect("PickUpItem");
                isUsed = true;
                BTN_E.SetActive(false);
                Cross.SetActive(false);
                ChangeSprite();
            }
        }
    }

    protected void ChangeSprite()
    {
        SellerRenderer.sprite = SellerAfterInteract;
    }
}