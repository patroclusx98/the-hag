using UnityEngine;

public class Climbable : MonoBehaviour
{
    [Header("Climbable Attributes")]
    [Range(0f, 1f)]
    public float faceAwayTolerance = 0.5f;

    private Player player;
    private bool canClimb;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (canClimb)
        {
            if (IsPlayerFacingClimbableObject())
            {
                player.isClimbing = true;
            }
            else
            {
                player.isClimbing = false;
            }
        }
    }

    /// <summary>
    /// Checks if the player is facing the climbable object
    /// </summary>
    /// <returns>True if facing is within the allowed tolerance</returns>
    private bool IsPlayerFacingClimbableObject()
    {
        Vector3 playerFacing = player.transform.right;
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
            player.isClimbing = false;
        }
    }
}
