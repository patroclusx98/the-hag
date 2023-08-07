using UnityEngine;

public class Climbable : MonoBehaviour
{
    PlayerMovement playerMovement;
    bool canClimb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (canClimb)
        {
            float facingDotProduct = CalcFacingDotProduct();

            if (facingDotProduct > 0.6f)
            {
                playerMovement.isClimbing = true;
            }
            else
            {
                playerMovement.isClimbing = false;
            }
        }
    }

    float CalcFacingDotProduct()
    {
        Vector3 ladderVector = -gameObject.transform.right;
        Vector3 cameraVector = Camera.main.transform.right;

        return Vector3.Dot(cameraVector, ladderVector);
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
