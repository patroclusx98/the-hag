using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public PlayerStats playerStats;
    public PlayerLook playerLook;

    [Header("Inventory UI Inspector")]
    [ReadOnlyInspector]
    public bool isInInventory;

    private PlayerInventory inventory;
    private InventorySlot[] slots;

    // Start is called before the first frame update
    private void Start()
    {
        inventory = PlayerInventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = inventoryUI.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (isInInventory || playerStats.canInteract))
        {
            ToggleInventory();
        }
    }

    private void UpdateUI()
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

    public void ToggleInventory(bool isItemSelected = false)
    {
        isInInventory = !isInInventory;
        inventoryUI.SetActive(isInInventory);

        playerStats.canInteract = !isInInventory && !isItemSelected;
        Cursor.lockState = isInInventory ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInInventory;
    }
}
