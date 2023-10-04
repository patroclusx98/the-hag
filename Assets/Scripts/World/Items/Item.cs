using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    [Range(0.01f, 10f)]
    public float iconScale = 1f;

    [HideInInspector]
    public List<GameObject> usableGameObjects;

    /// <summary>
    /// Defines the logic of which an item can be used by
    /// </summary>
    /// <param name="gameObject">Game object to use the item on</param>
    /// <returns>True if the item was successfully used</returns>
    public virtual bool Use(GameObject gameObject)
    {
        if (usableGameObjects.Count == 0)
        {
            /** Item does not need a game object to be used on **/

            return true;
        }
        else
        {
            /** Item needs a game object to be used on **/

            if (gameObject != null)
            {
                if (usableGameObjects.Contains(gameObject))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
