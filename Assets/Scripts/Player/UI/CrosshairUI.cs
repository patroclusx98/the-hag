using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Camera mainCamera;
    public RawImage crosshair;
    public RawImage crosshairHand;
    public Image crosshairItem;
    public Animator crosshairItemAnimator;

    // Update is called once per frame
    void Update()
    {
        bool raycast = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        Item selectedItem = PlayerInventory.instance != null ? PlayerInventory.instance.selectedItem : null;
        if (selectedItem != null)
        {
            crosshairHand.enabled = false;
            crosshair.enabled = false;

            crosshairItem.enabled = true;
            crosshairItem.sprite = selectedItem.icon;

            if (raycast && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshairItemAnimator.SetBool("isItemOverObject", true);
            }
            else
            {
                crosshairItemAnimator.SetBool("isItemOverObject", false);
            }
        }
        else
        {
            if (crosshairItem.enabled)
            {
                crosshairItemAnimator.SetBool("isItemOverObject", false);
                crosshairItem.enabled = false;
            }

            if (playerStats.canInteract && raycast && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshairHand.enabled = true;
                crosshair.enabled = false;
            }
            else
            {
                crosshairHand.enabled = false;
                crosshair.enabled = true;
            }
        }
    }
}
