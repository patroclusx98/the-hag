using UnityEngine;

public class PlayerBobbing : MonoBehaviour
{
    public Player player;

    [Header("Bobbing Attributes")]
    public float bobbingAmount = 0.03f;
    public float walkingBobbingSpeed = 6f;
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
        if (player.IsMoving() && (Mathf.Abs(player.horizontalVelocity.x) > 0.35f || Mathf.Abs(player.horizontalVelocity.z) > 0.35f))
        {
            /** Player is moving **/

            float bobbingSpeed = player.isRunning ? runningBobbingSpeed : walkingBobbingSpeed * player.movementSpeed;

            /** Update the main camera's X and Y position to mimic bobbing **/
            timer += Time.deltaTime * bobbingSpeed;
            transform.localPosition = new Vector3(defaultPos.x + Mathf.Sin(timer * 0.5f) * bobbingAmount, defaultPos.y + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            /** Player is idle **/

            /** Reset the main camera's X and Y position to default **/
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
