using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Inventory/Key")]
public class ItemKey : Item
{
    public override bool Use(GameObject hoveredObject)
    {
        if (base.Use(hoveredObject))
        {
            hoveredObject.TryGetComponent<DoorInteractable>(out var doorObject);
            if (doorObject != null)
            {
                doorObject.isLocked = false;
                PlayerInventory.instance.RemoveItem(this);
                return true;
            }
        }

        return false;
    }
}
