using UnityEngine;
using System.Collections.Generic;

public class ItemInteractable : MonoBehaviour
{
    public Item item;
    public List<GameObject> usableGameObjects;

    // Reset is called on component add/reset
    private void Reset()
    {
        /** Automatically set game object parameters **/
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
