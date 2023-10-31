using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public Player player;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (player.playerInventory.selectedItem != null && player.CanInteractWith(Player.Interaction.Item))
            {
                UseSelectedItem();
            }
            else if (player.CanInteract())
            {
                PickUpItem();
            }
        }
    }

    /// <summary>
    /// Uses the item actively selected from the inventory by calling the item's use method
    /// </summary>
    private void UseSelectedItem()
    {
        bool rayHit = Physics.Raycast(player.playerLook.transform.position, player.playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        bool itemUsed = player.playerInventory.selectedItem.Use(rayHit ? hitInfo.transform.gameObject : null);

        if (itemUsed)
        {
            player.playerInventory.RemoveItem(player.playerInventory.selectedItem);
        }
        else
        {
            player.playerInventory.selectedItem = null;
            HintUI.instance.DisplayHintMessage("Could not use item!");
        }

        player.modifiers.Remove(Player.Modifier.Interacting);
    }

    /// <summary>
    /// Attempts to pick up the item the player is looking at and place it in the player's inventory
    /// </summary>
    private void PickUpItem()
    {
        bool rayHit = Physics.Raycast(player.playerLook.transform.position, player.playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            if (hitInfo.transform.gameObject.TryGetComponent(out ItemInteractable itemObject))
            {
                if (player.playerInventory.HasSpace())
                {
                    player.playerInventory.AddItem(itemObject.item);
                    Destroy(itemObject.gameObject);
                }
                else
                {
                    HintUI.instance.DisplayHintMessage("No space in inventory!");
                }
            }
            else
            {
                Debug.LogWarning("Interactable Item object does not hold an ItemInteractable component: " + hitInfo.transform.gameObject.name);
            }
        }
    }
}
