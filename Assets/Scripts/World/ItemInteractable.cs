using UnityEngine;
using System.Collections.Generic;

public class ItemInteractable : MonoBehaviour
{
    public Item item;
    public List<GameObject> usableGameObjects = new List<GameObject>();

    //Reset is called on component add/reset
    private void Reset()
    {
        //Auto set item object params
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
