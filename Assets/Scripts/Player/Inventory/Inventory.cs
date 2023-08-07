using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> itemsList = new List<Item>();

    [HideInInspector]
    public Item selectedItem;

    public int maxSpace = 10;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Awake()
    {
        //Singleton instance
        if (instance != null)
            Debug.LogWarning("More than one instance of Inventory found!");

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

    public Item GetItem(int index)
    {
        if (itemsList.Count > index)
        {
            return itemsList[index];
        }
        else
        {
            Debug.Log("Invalid inventory item index!");
            return null;
        }
    }

    public bool HasSpace()
    {
        if (itemsList.Count < maxSpace)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
