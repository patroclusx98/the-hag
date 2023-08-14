using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Camera mainCamera;

    [Header("FOV Attributes")]
    public float distortionSensitivity = 3f;

    private float defaultFov;

    // Start is called before the first frame update
    private void Start()
    {
        defaultFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerMovement.isRunning || playerMovement.verticalVelocity.y < -10f)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov + 15f, distortionSensitivity * Time.deltaTime);
        }
        else
        {
            if (mainCamera.fieldOfView > 60.01f)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov, distortionSensitivity * Time.deltaTime);
            }
        }
    }
}
