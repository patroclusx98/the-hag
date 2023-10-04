﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerLook playerLook;
    public Image crosshairImage;
    public Animator crosshairAnimator;

    [Header("Crosshair Sprites")]
    public Crosshair[] crosshairList;

    private Dictionary<string, Crosshair> crosshairDict = new Dictionary<string, Crosshair>();

    // Awake is called on script load
    private void Awake()
    {
        foreach (Crosshair crosshair in crosshairList)
        {
            crosshairDict.Add(crosshair.name, crosshair);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        Item selectedItem = PlayerInventory.instance != null ? PlayerInventory.instance.selectedItem : null;

        if (selectedItem != null)
        {
            /** Active item selection from inventory **/

            /** Set crosshair to the selected item **/
            crosshairImage.sprite = selectedItem.icon;
            crosshairImage.transform.localScale = Vector3.one * selectedItem.iconScale;

            /** Set crosshair pulsing animation based on player look objects **/
            if (rayHit && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshairAnimator.SetBool("SetPulseCrosshair", true);
            }
            else
            {
                crosshairAnimator.SetBool("SetPulseCrosshair", false);
            }
        }
        else
        {
            /** No active item selection from inventory **/

            Crosshair crosshair;
            crosshairAnimator.SetBool("SetPulseCrosshair", false);

            /** Set crosshair based on player look objects **/
            if (playerStats.canInteract && rayHit && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshair = crosshairDict["crosshair_hand"];
            }
            else
            {
                crosshair = crosshairDict["crosshair"];
            }

            crosshairImage.sprite = crosshair.sprite;
            crosshairImage.transform.localScale = Vector3.one * crosshair.scale;
        }
    }
}
