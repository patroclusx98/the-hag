using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Bobbing Attributes")]
    public float bobbingAmount = 0.04f;
    public float walkingBobbingSpeed = 12.5f;
    public float runningBobbingSpeed = 18f;

    private float defaultPosX;
    private float defaultPosY;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        defaultPosX = transform.localPosition.x;
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerMovement.IsPlayerMoving() && (Mathf.Abs(playerMovement.moveVelocity.x) > 0.35f || Mathf.Abs(playerMovement.moveVelocity.z) > 0.35f))
        {
            //Player is moving
            timer += Time.deltaTime * (playerMovement.isRunning ? runningBobbingSpeed : walkingBobbingSpeed * playerMovement.playerSpeed * 0.5f);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
            transform.localPosition = new Vector3(defaultPosX + Mathf.Sin(timer * 0.5f) * bobbingAmount, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            //Player is idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * runningBobbingSpeed), transform.localPosition.z);
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, defaultPosX, Time.deltaTime * runningBobbingSpeed), transform.localPosition.y, transform.localPosition.z);
        }
    }

    public void UpdateDefaultPosY(float newDefaultPosY)
    {
        defaultPosY = newDefaultPosY;
    }
}
