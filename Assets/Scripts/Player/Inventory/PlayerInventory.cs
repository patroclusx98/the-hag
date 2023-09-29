using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    [Header("Inventory Attributes")]
    public int maxSpace = 8;
    public List<Item> itemsList = new List<Item>();

    [Header("Inventory Inspector")]
    [ReadOnlyInspector]
    public Item selectedItem;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // Awake is called on script load
    private void Awake()
    {
        /** Singleton instance **/

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Player Inventory is found!");
        }

        instance = this;
    }

    public void AddItem(Item itemObject)
    {
        itemsList.Add(itemObject);
        onItemChangedCallback?.Invoke();
    }

    public void RemoveItem(Item itemObject)
    {
        itemsList.Remove(itemObject);
        onItemChangedCallback?.Invoke();
    }

    public void RemoveAll()
    {
        itemsList.Clear();
        onItemChangedCallback?.Invoke();
    }

    public Item GetItem(int index)
    {
        if (itemsList.Count > index)
        {
            return itemsList[index];
        }
        else
        {
            Debug.LogWarning("Invalid inventory item index!");

            return null;
        }
    }

    public bool HasSpace()
    {
        return itemsList.Count < maxSpace;
    }
}
