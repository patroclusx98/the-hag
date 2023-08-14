using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public Camera mainCamera;

    private PlayerInventory inventory;
    private GameObject itemInHand;

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

            if (selectedItem != null)
            {
                bool rayHit = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
                selectedItem.Use(rayHit ? hitInfo.transform.gameObject : null);
                inventory.selectedItem = null;
                playerStats.canInteract = true;
            }
            else
            {
                if (playerStats.canInteract)
                {
                    PickUpItem();
                }
            }
        }
    }

    private void PickUpItem()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Item"), QueryTriggerInteraction.Ignore))
        {
            itemInHand = hitInfo.transform.gameObject;

            if (itemInHand.CompareTag("Interactable"))
            {
                if (inventory.HasSpace())
                {
                    ItemInteractable itemObjHolder = itemInHand.GetComponent<ItemInteractable>();
                    Item item = itemObjHolder != null ? itemObjHolder.item : null;
                    if (item != null)
                    {
                        item.usableGameObjects = itemObjHolder.usableGameObjects;
                        inventory.AddItem(item);
                        Destroy(itemInHand);
                    }
                    else
                    {
                        Debug.LogWarning("GameObject does not hold an Item object!");
                    }
                }
                else
                {
                    HintUI.instance.DisplayHintMessage("No space in inventory!");
                }
            }
        }
    }
}
