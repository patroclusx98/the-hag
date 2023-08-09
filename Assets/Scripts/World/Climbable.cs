using UnityEngine;

public class Climbable : MonoBehaviour
{
    [Header("Climbable Attributes")]
    [Range(0f, -1f)]
    public float faceAwayTolerance = -0.5f;

    private PlayerMovement playerMovement;
    private bool canClimb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canClimb)
        {
            if (CheckFacing())
            {
                playerMovement.isClimbing = true;
            }
            else
            {
                playerMovement.isClimbing = false;
            }
        }
    }

    //Check if player is facing the climbable object
    bool CheckFacing()
    {
        Vector3 objectFacing = -gameObject.transform.right;
        Vector3 cameraFacing = Camera.main.transform.right;
        float facingDotProduct = Vector3.Dot(cameraFacing, objectFacing);

        return facingDotProduct > faceAwayTolerance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canClimb = false;
            playerMovement.isClimbing = false;
        }
    }
}
