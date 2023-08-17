using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void SelectItem()
    {
        if (item != null)
        {
            if (item.usableGameObjects.Count == 0)
            {
                item.Use(null);
            }
            else
            {
                PlayerInventory.instance.selectedItem = item;
            }

            GetComponentInParent<InventoryUI>().ToggleInventory(true);
        }
    }
}
