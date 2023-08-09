using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public PlayerStats playerStats;
    public MouseLook mouseLook;

    private PlayerInventory inventory;
    private InventorySlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = PlayerInventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = inventoryUI.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((playerStats.canInteract || mouseLook.isInInventory) && Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory(false);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.itemsList.Count)
            {
                slots[i].AddItem(inventory.itemsList[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void ToggleInventory(bool isItemSelected)
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        mouseLook.ToggleInventoryCursor(isItemSelected);
    }
}
