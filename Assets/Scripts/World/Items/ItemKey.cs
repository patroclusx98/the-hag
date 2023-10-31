using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Item/Key")]

public class ItemKey : Item
{
    public override bool Use(GameObject gameObject)
    {
        if (base.Use(gameObject))
        {
            if (gameObject.TryGetComponent<DoorInteractable>(out var doorObject))
            {
                doorObject.isLocked = false;

                return true;
            }
        }

        return false;
    }
}
