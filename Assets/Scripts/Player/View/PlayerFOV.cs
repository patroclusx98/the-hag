using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    public Player player;
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
        if ((player.isRunning && player.horizontalVelocity.magnitude > 0.5f) || player.verticalVelocity.y < player.fallDamageTolerance)
        {
            /** Player is running or falling **/

            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFov + distortionAmount, distortionSpeed * Time.deltaTime);
        }
        else
        {
            /** Player is not running and not falling **/

            /** Reset the main camera's fov **/
            if (mainCamera.fieldOfView > defaultFov + 0.01f)
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
