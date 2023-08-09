using System;
using UnityEngine;

public class FrequencyTimer : MonoBehaviour
{
    private string ID;
    private float frequencyInSeconds;
    private float timeElapsedWhileMute;
    private float frequencyCounter;
    private bool hasStarted;
    private bool hasEnded;

    public static FrequencyTimer GetInstance(string ID, GameObject gameObject)
    {
        string prefixID = gameObject.name + "_";

        FrequencyTimer ft = Array.Find(gameObject.GetComponents<FrequencyTimer>(), instance => instance.ID == prefixID + ID);

        if (ft == null)
        {
            ft = gameObject.AddComponent<FrequencyTimer>();
            ft.ID = prefixID + ID;
        }
        return ft;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            timeElapsedWhileMute = 0;
            CountFreq();
        }
        else
        {
            timeElapsedWhileMute += Time.deltaTime;
        }

        if (timeElapsedWhileMute > frequencyInSeconds)
        {
            Destroy(this);
        }
    }

    void CountFreq()
    {
        if (frequencyCounter < frequencyInSeconds)
        {
            frequencyCounter += Time.deltaTime * 0.9f;
        }
        else
        {
            frequencyCounter = 0f;
            hasStarted = false;
            hasEnded = true;
        }
    }

    public bool IsStartPhase(float frequencyInSeconds)
    {
        if (!hasStarted)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasStarted = true;
            hasEnded = false;

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsEndPhase(float frequencyInSeconds)
    {
        if (!hasStarted)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasStarted = true;
        }

        if (hasEnded)
        {
            hasEnded = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
