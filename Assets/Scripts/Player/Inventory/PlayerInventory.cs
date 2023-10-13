using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Attributes")]
    public int maxSpace = 8;
    public List<Item> itemList;

    [Header("Inventory Inspector")]
    [ReadOnlyInspector]
    public Item selectedItem;

    public delegate void OnInventoryChange();
    public OnInventoryChange onInventoryChange;

    /// <summary>
    /// Checks if there are any free spaces in the inventory
    /// </summary>
    /// <returns>True if inventory has free space</returns>
    public bool HasSpace()
    {
        return itemList.Count < maxSpace;
    }

    /// <summary>
    /// Adds the given item to the inventory
    /// </summary>
    /// <param name="itemObject">The item object to add to the inventory</param>
    public void AddItem(Item itemObject)
    {
        itemList.Add(itemObject);
        onInventoryChange?.Invoke();
    }

    /// <summary>
    /// Removes the given item from the inventory
    /// </summary>
    /// <param name="itemObject">The item object to remove from the inventory</param>
    public void RemoveItem(Item itemObject)
    {
        itemList.Remove(itemObject);
        selectedItem = (selectedItem == itemObject) ? null : selectedItem;
        onInventoryChange?.Invoke();
    }

    /// <summary>
    /// Removes all of the items from the inventory
    /// </summary>
    public void RemoveAllItems()
    {
        itemList.Clear();
        selectedItem = null;
        onInventoryChange?.Invoke();
    }

    /// <summary>
    /// Gets the item object in the inventory by it's index
    /// </summary>
    /// <param name="index">The inventory index of the item</param>
    /// <returns>The item object if found, otherwise null</returns>
    public Item GetItem(int index)
    {
        if (itemList.Count > index)
        {
            return itemList[index];
        }
        else
        {
            Debug.LogWarning("Invalid inventory item index!");

            return null;
        }
    }
}
