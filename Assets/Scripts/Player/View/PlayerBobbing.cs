using UnityEngine;

public class PlayerBobbing : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Bobbing Attributes")]
    public float bobbingAmount = 0.03f;
    public float walkingBobbingSpeed = 12.5f;
    public float runningBobbingSpeed = 18f;

    private Vector3 defaultPos;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        defaultPos = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerMovement.IsPlayerMoving() && (Mathf.Abs(playerMovement.horizontalVelocity.x) > 0.35f || Mathf.Abs(playerMovement.horizontalVelocity.z) > 0.35f))
        {
            /** Player is moving **/

            float bobbingSpeed = playerMovement.isRunning ? runningBobbingSpeed : walkingBobbingSpeed * playerMovement.playerSpeed * 0.5f;

            timer += Time.deltaTime * bobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPos.y + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
            transform.localPosition = new Vector3(defaultPos.x + Mathf.Sin(timer * 0.5f) * bobbingAmount, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            /** Player is idle **/

            if (Vector3.Distance(transform.localPosition, defaultPos) > 0.00001f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPos, Time.deltaTime * runningBobbingSpeed);
            }
            else
            {
                transform.localPosition = defaultPos;
            }

            timer = 0f;
        }
    }
}
