using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    public Animator doorHandleAnimator;

    [Header("Door Attributes")]
    public float dragResistanceMouse = 0.5f;
    public float dragResistanceWalk = 60f;
    [Range(80f, 160f)]
    public float maxOpeningDegrees = 120f;
    public bool isLeftSided;
    public bool isLocked;
    public Quaternion defaultClosedRotation;

    [Header("Door Inspector Basic")]
    [ReadOnlyInspector]
    public bool isDoorGrabbed;
    [ReadOnlyInspector]
    public bool isPlayerColliding;
    [ReadOnlyInspector]
    public bool isSlammed;

    [Header("Door Inspector Advanced")]
    [ReadOnlyInspector]
    public float yDegMotion;
    [ReadOnlyInspector]
    public float lastYDegMotion;
    [ReadOnlyInspector]
    public Quaternion fromRotation;
    [ReadOnlyInspector]
    public Quaternion toRotation;
    [ReadOnlyInspector]
    public float motionVelocity;
    [ReadOnlyInspector]
    public float physicsVelocity;
    [ReadOnlyInspector]
    public float lerpTimer;
    [ReadOnlyInspector]
    public IEnumerator prevCoroutine;

    //Reset is called on component add/reset
    private void Reset()
    {
        //Auto set door params
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Door");
        defaultClosedRotation = gameObject.transform.parent.rotation;

        //Auto add trigger collider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (gameObject.GetComponent<BoxCollider>() != null)
            DestroyImmediate(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(boxCollider.size.x + 0.1f, boxCollider.size.y, boxCollider.size.z + 0.2f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isDoorGrabbed && prevCoroutine == null && !IsDoorClosed(0f) && IsDoorClosed(1f))
        {
            fromRotation = gameObject.transform.rotation;
            toRotation = Quaternion.Euler(gameObject.transform.eulerAngles.x, defaultClosedRotation.eulerAngles.y, gameObject.transform.eulerAngles.z);

            gameObject.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, 0.1f);
        }
    }

    //Check if door is near to closed position by specified amount
    //Passing '0f' will check for fully closed state
    public bool IsDoorClosed(float deviationInDegrees)
    {
        float angle = Quaternion.Angle(gameObject.transform.rotation, defaultClosedRotation);

        return angle <= deviationInDegrees;
    }

    //Clamps rotation to min and max door openings
    public float ClampRotation(float rotation)
    {
        //Calculate motion velocity
        float calcDifference = rotation - lastYDegMotion;
        if (calcDifference < -maxOpeningDegrees * 2f)
        {
            calcDifference += 360f;
        }
        else if (calcDifference > maxOpeningDegrees * 2f)
        {
            calcDifference -= 360f;
        }
        motionVelocity = Mathf.Clamp(calcDifference, -2f, 2f);

        //Check and clamp rotation
        if (isLeftSided)
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation - maxOpeningDegrees;
            float toRotation = calcMaxRotation < 0f ? 360f + calcMaxRotation : calcMaxRotation;

            //Negative rotation
            if (fromRotation > toRotation)
            {
                if (rotation <= fromRotation && rotation >= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, toRotation, fromRotation);
                }
            }
            else
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;
                //ISSUE: R0-LY359(E-359 SE1) || R359+LY1(E360 SE0)

                if (!(wrappedRotation > fromRotation && wrappedRotation < toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation > fromRotation && motionVelocity > 0f)
                {
                    return fromRotation;
                }
                else if (rotation < toRotation && motionVelocity < 0f)
                {
                    return toRotation;
                }
            }
        }
        else
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation + maxOpeningDegrees;
            float toRotation = calcMaxRotation >= 360f ? calcMaxRotation - 360f : calcMaxRotation;

            //Positive rotation
            if (fromRotation > toRotation)
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;

                if (!(wrappedRotation < fromRotation && wrappedRotation > toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation < fromRotation && motionVelocity < 0f)
                {
                    return fromRotation;
                }
                else if (rotation > toRotation && motionVelocity > 0f)
                {
                    return toRotation;
                }
            }
            else
            {
                if (rotation >= fromRotation && rotation <= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, fromRotation, toRotation);
                }
            }
        }

        return rotation;
    }

    //Calculate the rotation from the velocity
    private void CalcVelocityToRotation(float multiplier)
    {
        fromRotation = gameObject.transform.rotation;
        float toRotationYVelocity = ClampRotation(fromRotation.eulerAngles.y + physicsVelocity * multiplier);
        if (toRotationYVelocity != fromRotation.eulerAngles.y)
        {
            toRotation = Quaternion.Euler(gameObject.transform.eulerAngles.x, toRotationYVelocity, gameObject.transform.eulerAngles.z);
        }

        physicsVelocity = 0f;
        lerpTimer = 0f;
    }

    //Moves the door by the applied velocity
    private IEnumerator MoveDoorByVelocity(bool useSmoothing)
    {
        while (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / (useSmoothing ? 1.4f : 0.8f);
            gameObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1 - Mathf.Pow(1 - lerpTimer, 3));

            yield return null;
        }

        isSlammed = false;
        prevCoroutine = null;
    }

    //Apply velocity to door
    public void ApplyVelocityToDoor(float multiplier)
    {
        if (physicsVelocity != 0f)
        {
            CalcVelocityToRotation(multiplier);

            if (prevCoroutine == null)
            {
                if (isSlammed)
                {
                    prevCoroutine = MoveDoorByVelocity(false);
                }
                else
                {
                    prevCoroutine = MoveDoorByVelocity(true);
                }
                StartCoroutine(prevCoroutine);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Stop any door motion
            if (prevCoroutine != null)
            {
                StopCoroutine(prevCoroutine);
                prevCoroutine = null;
            }
            physicsVelocity = 0f;

            isPlayerColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = false;
        }
    }

    //EVENT HANDLERS FOR OTHER SCRIPTS

    public void EventLockDoor()
    {
        isLocked = true;
    }

    public void EventUnlockDoor()
    {
        isLocked = false;
    }

    public void EventApplyVelocityToDoor(float velocity)
    {
        if (velocity != 0)
        {
            physicsVelocity = velocity;
            ApplyVelocityToDoor(1f);
        }
    }

    public void EventSlamDoor(bool isOpening)
    {
        isSlammed = true;

        if (isOpening)
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
        else
        {
            physicsVelocity = isLeftSided ? -maxOpeningDegrees : maxOpeningDegrees;
        }

        ApplyVelocityToDoor(1f);
    }
}
