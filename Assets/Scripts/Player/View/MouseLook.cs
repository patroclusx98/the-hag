using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;

    [Header("Mouse Attributes")]
    [Range(0.3f, 3f)]
    public float mouseSensitivity = 1f;

    [Header("Mouse Inspector")]
    [ReadOnlyInspector]
    public bool isInteracting;
    [ReadOnlyInspector]
    public bool isInInventory;
    [ReadOnlyInspector]
    public float xRotation;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isInteracting && !isInInventory)
        {
            DoMouseLook();
        }
    }

    private void DoMouseLook()
    {
        //Looking logic
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (mouseX != 0 || mouseY != 0)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 70f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerMovement.transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void ToggleInventoryCursor(bool isItemSelected)
    {
        isInInventory = !isInInventory;

        if (isInInventory)
        {
            playerStats.canInteract = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (!isItemSelected)
                playerStats.canInteract = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
