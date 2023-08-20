using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public PlayerLook playerLook;
    public ObjectInteraction objectInteraction;

    private Quaternion defaultTransform;

    // Start is called before the first frame update
    void Start()
    {
        defaultTransform = gameObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerLook.isInteracting && !playerLook.isInInventory)
        {
            HandSway();
        }
    }

    void HandSway()
    {
        float mouseX = Input.GetAxis("Mouse X") * playerLook.mouseSensitivity;
        float xMouseRotation = -playerLook.xRotation;
        float upLimit = objectInteraction.carryingObject ? 75f : 40f;
        float downLimit = objectInteraction.carryingObject ? -40f : -50f;

        if (xMouseRotation > upLimit)
        {
            /** Looking upwards **/

            xMouseRotation -= upLimit;
        }
        else if (xMouseRotation < downLimit)
        {
            /** Looking downwards **/

            xMouseRotation -= downLimit;
        }
        else
        {
            /** Looking straight **/

            xMouseRotation = 0f;
        }

        Quaternion toTransform = defaultTransform * Quaternion.Euler(xMouseRotation, 0f, 0f) * Quaternion.AngleAxis(-mouseX * 1.5f, Vector2.up);
        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, toTransform, 10f * Time.deltaTime);
    }
}