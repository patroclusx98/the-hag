﻿using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public RawImage crosshair;
    public RawImage crosshairHand;
    public Image crosshairItem;
    public Animator crosshairItemAnimator;

    // Update is called once per frame
    void Update()
    {
        bool raycast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, PlayerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        Item selectedItem = Inventory.instance != null ? Inventory.instance.selectedItem : null;
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

            if (PlayerStats.canInteract && raycast && hitInfo.transform.CompareTag("Interactable"))
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
