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

    /// <summary>
    /// Rotates the player's hand to follow the rotation of the player's head
    /// <para>This is to ensure player interactions follow changes of the player head's rotation</para>
    /// </summary>
    private void HandSway()
    {
        float handRotationX = Mathf.Clamp(playerLook.headXRotation, minHandXRotation, maxHandXRotation);
        float handRotationY = playerLook.headYRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-handRotationX, handRotationY, 0f), handSwaySpeed * Time.deltaTime);
    }
}