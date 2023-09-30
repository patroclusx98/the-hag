using UnityEngine;

public class Climbable : MonoBehaviour
{
    [Header("Climbable Attributes")]
    [Range(0f, 1f)]
    public float faceAwayTolerance = 0.5f;

    private PlayerMovement playerMovement;
    private bool canClimb;

    // Start is called before the first frame update
    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (canClimb)
        {
            if (IsPlayerFacingClimbableObject())
            {
                playerMovement.isClimbing = true;
            }
            else
            {
                playerMovement.isClimbing = false;
            }
        }
    }

    /// <summary>
    /// Checks if the player is facing the climbable object
    /// </summary>
    /// <returns>True if facing is within the allowed tolerance</returns>
    private bool IsPlayerFacingClimbableObject()
    {
        Vector3 playerFacing = playerMovement.transform.right;
        Vector3 objectFacing = -gameObject.transform.right;
        float facingDotProduct = Vector3.Dot(playerFacing, objectFacing);

        return facingDotProduct > -faceAwayTolerance;
    }

    /// <summary>
    /// Runs when the player's collider enters the climbable object's trigger collider
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canClimb = true;
        }
    }

    /// <summary>
    /// Runs when the player's collider leaves the climbable object's trigger collider
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canClimb = false;
            playerMovement.isClimbing = false;
        }
    }
}
