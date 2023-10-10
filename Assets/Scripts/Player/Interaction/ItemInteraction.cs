using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;

    private PlayerInventory playerInventory;

    // Start is called before the first frame update
    private void Start()
    {
        playerInventory = PlayerInventory.instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (playerInventory != null && playerInventory.selectedItem != null && player.CanInteractWith(Player.Interaction.Item))
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
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        playerInventory.selectedItem.Use(rayHit ? hitInfo.transform.gameObject : null);
        playerInventory.selectedItem = null;

        player.modifiers.Remove(Player.Modifier.Interacting);
    }

    /// <summary>
    /// Attempts to pick up the item the player is looking at and place it in the player's inventory
    /// </summary>
    private void PickUpItem()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            ItemInteractable itemObject = hitInfo.transform.gameObject.GetComponent<ItemInteractable>();
            Item item = itemObject != null ? itemObject.item : null;

            if (item != null)
            {
                if (playerInventory.HasSpace())
                {
                    item.usableGameObjects = itemObject.usableGameObjects;
                    playerInventory.AddItem(item);

                    Destroy(itemObject.gameObject);
                }
                else
                {
                    HintUI.instance.DisplayHintMessage("No space in inventory!");
                }
            }
            else
            {
                Debug.LogWarning("GameObject does not hold an Item component: " + itemObject.name);
            }
        }
    }
}
