using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public ObjectInteraction objectInteraction;
    public MouseLook mouseLook;

    private Quaternion originTransform;

    // Start is called before the first frame update
    void Start()
    {
        originTransform = gameObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mouseLook.isInteracting && !mouseLook.isInInventory)
        {
            HandSway();
        }
    }

    void HandSway()
    {
        float upLimit = objectInteraction.carryingObject ? 75f : 40f;
        float downLimit = objectInteraction.carryingObject ? -40f : -50f;
        float xMouseRotation = -mouseLook.xRotation;
        float mouseX = Input.GetAxis("Mouse X") * mouseLook.mouseSensitivity;

        if (xMouseRotation > upLimit)
        {
            //Looking upwards
            xMouseRotation -= upLimit;
        }
        else if (xMouseRotation < downLimit)
        {
            //Looking downwards
            xMouseRotation -= downLimit;
        }
        else
        {
            xMouseRotation = 0f;
        }

        Quaternion toTransform = originTransform * Quaternion.Euler(xMouseRotation, 0f, 0f) * Quaternion.AngleAxis(-mouseX * 1.5f, Vector2.up);
        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, toTransform, Time.deltaTime * 4f);
    }
}
