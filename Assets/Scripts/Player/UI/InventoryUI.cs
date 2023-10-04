using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public PlayerStats playerStats;
    public PlayerLook playerLook;

    [Header("Inventory UI Inspector")]
    [ReadOnlyInspector]
    public bool isInInventory;

    private PlayerInventory playerInventory;
    private InventorySlot[] inventorySlots;

    // Start is called before the first frame update
    private void Start()
    {
        playerInventory = PlayerInventory.instance;
        playerInventory.onInventoryChange += UpdateInventoryUI;

        inventorySlots = inventoryUI.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (isInInventory || playerStats.canInteract))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Updates the inventory slots when the inventory changes
    /// </summary>
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < playerInventory.itemsList.Count)
            {
                inventorySlots[i].AddItem(playerInventory.itemsList[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// Toggles the inventory on or off
    /// </summary>
    public void ToggleInventory()
    {
        isInInventory = !isInInventory;

        if (isInInventory)
        {
            inventoryUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerStats.canInteract = false;
            playerLook.LockMouseLook();
        }
        else
        {
            inventoryUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerStats.canInteract = !playerInventory.selectedItem;
            playerLook.UnlockMouseLook();
        }
    }
}
