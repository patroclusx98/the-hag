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
    private void Update()
    {
        bool rayHit = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        Item selectedItem = PlayerInventory.instance != null ? PlayerInventory.instance.selectedItem : null;

        if (selectedItem != null)
        {
            /** Active item selection from inventory **/

            crosshair.enabled = false;
            crosshairHand.enabled = false;

            crosshairItem.enabled = true;
            crosshairItem.sprite = selectedItem.icon;

            if (rayHit && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshairItemAnimator.SetBool("PulseCrosshairItem", true);
            }
            else
            {
                crosshairItemAnimator.SetBool("PulseCrosshairItem", false);
            }
        }
        else
        {
            /** No active item selection from inventory **/

            if (crosshairItem.enabled)
            {
                crosshairItemAnimator.SetBool("PulseCrosshairItem", false);
                crosshairItem.enabled = false;
            }

            if (playerStats.canInteract && rayHit && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshair.enabled = false;
                crosshairHand.enabled = true;
            }
            else
            {
                crosshair.enabled = true;
                crosshairHand.enabled = false;
            }
        }
    }
}
