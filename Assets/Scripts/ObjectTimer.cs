using System;
using UnityEngine;

public class ObjectTimer : MonoBehaviour
{
    private string id;
    private float timeCounter;

    /// <summary>
    /// Attaches a timer with a unique ID to the defined game object and starts it
    /// </summary>
    /// <param name="id">Unique ID of this timer instance</param>
    /// <param name="gameObject">Game object to attach the timer to</param>
    /// <param name="time">Time in seconds to set the timer to</param>
    /// <returns>True on timer start, False while timer is progressing</returns>
    public static bool StartTimer(string id, GameObject gameObject, float time)
    {
        string prefixedId = gameObject.name + "_" + id;
        ObjectTimer objectTimer = Array.Find(gameObject.GetComponents<ObjectTimer>(), (objectTimer) => objectTimer.id == prefixedId);

        if (objectTimer == null)
        {
            objectTimer = gameObject.AddComponent<ObjectTimer>();
            objectTimer.id = prefixedId;
        }

        if (objectTimer.timeCounter == 0f)
        {
            objectTimer.timeCounter = time;
            return true;
        }

        return false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (timeCounter > 0f)
        {
            timeCounter -= Time.deltaTime;

            if (timeCounter <= 0f)
            {
                timeCounter = 0f;
            }
        }
    }
}
