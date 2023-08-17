using System;
using UnityEngine;

public class FrequencyTimer : MonoBehaviour
{
    private string id;
    private float frequencyTime;
    private float frequencyCounter;
    private float idleCounter;
    private bool hasStarted;

    public static FrequencyTimer GetInstance(string id, GameObject gameObject)
    {
        string prefixedId = gameObject.name + "_" + id;

        FrequencyTimer ft = Array.Find(gameObject.GetComponents<FrequencyTimer>(), instance => instance.id == prefixedId);

        if (ft == null)
        {
            ft = gameObject.AddComponent<FrequencyTimer>();
            ft.id = prefixedId;
        }

        return ft;
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasStarted)
        {
            CountFrequency();
        }
        else
        {
            CountIdle();
        }
    }

    private void CountFrequency()
    {
        frequencyCounter += Time.deltaTime;

        if (frequencyCounter >= frequencyTime)
        {
            frequencyCounter = 0f;
            idleCounter = 0f;
            hasStarted = false;
        }
    }

    private void CountIdle()
    {
        idleCounter += Time.deltaTime;

        if (idleCounter >= frequencyTime * 2f)
        {
            Destroy(this);
        }
    }

    // Returns true at the start of every phase
    public bool GetStartPhase(float frequencyTime)
    {
        if (!hasStarted)
        {
            this.frequencyTime = frequencyTime;
            hasStarted = true;

            return true;
        }

        return false;
    }
}
