using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Inventory/Key")]
public class ItemKey : Item
{
    public override bool Use(GameObject hoveredObject)
    {
        if (base.Use(hoveredObject))
        {
            hoveredObject.TryGetComponent<DoorInteraction>(out var doorInteraction);
            if (doorInteraction != null)
            {
                doorInteraction.isLocked = false;
                Inventory.instance.RemoveItem(this);
                return true;
            }
        }

        return false;
    }
}
