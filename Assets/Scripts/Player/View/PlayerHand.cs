using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerLook playerLook;

    [Header("Player Hand Attributes")]
    public float handSwaySpeed = 50f;
    public float minHandXRotation = -40f;
    public float maxHandXRotation = 75f;

    // Update is called once per frame
    private void Update()
    {
        HandSway();
    }

    private void HandSway()
    {
        float headRotationX = Mathf.Clamp(playerLook.headXRotation, minHandXRotation, maxHandXRotation);
        float headRotationY = playerLook.headYRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-headRotationX, headRotationY, 0f), handSwaySpeed * Time.deltaTime);
    }
}