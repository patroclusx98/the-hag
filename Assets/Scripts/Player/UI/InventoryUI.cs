using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Player player;
    public GameObject inventoryUI;

    [Header("Inventory UI Attributes")]
    public List<InventorySlot> inventorySlots;

    [Header("Inventory UI Inspector")]
    [ReadOnlyInspector]
    public bool isInventoryOpen;

    // Start is called before the first frame update
    private void Start()
    {
        player.playerInventory.onInventoryChange += UpdateInventoryUI;
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
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < player.playerInventory.itemList.Count)
            {
                inventorySlots[i].AddItem(player.playerInventory.itemList[i]);
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
            player.playerInventory.selectedItem = null;
            player.playerLook.LockMouseLook();
        }
        else
        {
            inventoryUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (player.playerInventory.selectedItem)
            {
                player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Item;
            }
            else
            {
                player.modifiers.Remove(Player.Modifier.Interacting);
            }

            player.playerLook.UnlockMouseLook();
        }
    }
}
