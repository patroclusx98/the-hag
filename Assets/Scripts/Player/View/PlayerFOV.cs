using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Camera mainCamera;

    [Header("FOV Attributes")]
    public float distortionAmount = 15f;
    public float distortionSpeed = 3f;

    private float defaultFov;

    // Start is called before the first frame update
    private void Start()
    {
        defaultFov = mainCamera.fieldOfView;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerMovement.isRunning || playerMovement.verticalVelocity.y < playerMovement.fallDamageTolerance)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov + distortionAmount, distortionSpeed * Time.deltaTime);
        }
        else
        {
            if (mainCamera.fieldOfView > defaultFov + 0.1f)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov, distortionSpeed * Time.deltaTime);
            }
            else
            {
                mainCamera.fieldOfView = defaultFov;
            }
        }
    }
}
