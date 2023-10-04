using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    private Item item;

    /// <summary>
    /// Adds the item to the inventory slot
    /// </summary>
    /// <param name="newItem">The item to add to the inventory slot</param>
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    /// <summary>
    /// Clears the inventory slot
    /// </summary>
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    /// <summary>
    /// Selects the item assigned to the inventory slot
    /// <para>This is called by the inventory slot's onClick</para>
    /// </summary>
    public void SelectItem()
    {
        if (item != null)
        {
            if (item.usableGameObjects.Count == 0)
            {
                /** Item does not need a game object to be used on **/

                item.Use(null);
            }
            else
            {
                /** Item needs a game object to be used on **/

                PlayerInventory.instance.selectedItem = item;
                GetComponentInParent<InventoryUI>().ToggleInventory();
            }
        }
    }
}
