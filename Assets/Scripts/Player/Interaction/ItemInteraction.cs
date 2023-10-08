using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;

    private PlayerInventory inventory;

    // Start is called before the first frame update
    private void Start()
    {
        inventory = PlayerInventory.instance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Item selectedItem = inventory != null ? inventory.selectedItem : null;

            if (selectedItem != null && player.CanInteractWith(Player.Interaction.Item))
            {
                UseSelectedItem(selectedItem);
            }
            else if (player.CanInteract())
            {
                PickUpItem();
            }
        }
    }

    private void UseSelectedItem(Item selectedItem)
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        selectedItem.Use(rayHit ? hitInfo.transform.gameObject : null);
        inventory.selectedItem = null;

        player.modifiers.Remove(Player.Modifier.Interacting);
    }

    private void PickUpItem()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            ItemInteractable itemObject = hitInfo.transform.gameObject.GetComponent<ItemInteractable>();
            Item item = itemObject != null ? itemObject.item : null;

            if (item != null)
            {
                if (inventory.HasSpace())
                {
                    item.usableGameObjects = itemObject.usableGameObjects;
                    inventory.AddItem(item);

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
