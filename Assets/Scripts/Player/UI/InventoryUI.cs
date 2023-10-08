using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;
    public GameObject inventoryUI;

    [Header("Inventory UI Inspector")]
    [ReadOnlyInspector]
    public bool isInventoryOpen;

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
        if (Input.GetKeyDown(KeyCode.E))
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
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Inventory;
            playerLook.LockMouseLook();
        }
        else
        {
            inventoryUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerInventory.selectedItem)
            {
                player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Item;
            }
            else
            {
                player.modifiers.Remove(Player.Modifier.Interacting);
            }

            playerLook.UnlockMouseLook();
        }
    }
}
