using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;

    GameObject itemInHand;
    Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Item selectedItem = inventory != null ? inventory.selectedItem : null;
            if (selectedItem != null)
            {
                RaycastHit hitInfo;
                bool rayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, PlayerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
                selectedItem.Use(rayHit ? hitInfo.transform.gameObject : null);

                inventory.selectedItem = null;
                PlayerStats.canInteract = true;
            }
            else
            {
                PickUpItem();
            }
        }
    }

    void PickUpItem()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore))
        {
            itemInHand = hitInfo.transform.gameObject;

            if (itemInHand.CompareTag("Interactable"))
            {
                if (Inventory.instance.HasSpace())
                {
                    ItemObjHolder itemObjHolder = itemInHand.GetComponent<ItemObjHolder>();
                    Item item = itemObjHolder != null ? itemObjHolder.item : null;
                    if (item != null)
                    {
                        Inventory.instance.AddItem(item);
                        item.usableGameObjects = itemObjHolder.usableGameObjects;
                        Destroy(itemInHand);
                    }
                    else
                    {
                        Debug.LogWarning("GameObject does not hold an Item object!");
                    }
                }
                else
                {
                    Debug.LogWarning("No space left in inventory!");
                }
            }
        }
    }
}
