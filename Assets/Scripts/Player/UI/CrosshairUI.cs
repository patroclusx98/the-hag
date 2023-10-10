using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;
    public Image crosshairImage;
    public Animator crosshairAnimator;

    [Header("Crosshair Sprites")]
    public Crosshair[] crosshairList;

    private PlayerInventory playerInventory;
    private Dictionary<string, Crosshair> crosshairDict = new Dictionary<string, Crosshair>();

    // Start is called before the first frame update
    private void Start()
    {
        playerInventory = PlayerInventory.instance;
    }

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
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        if (playerInventory != null && playerInventory.selectedItem != null)
        {
            /** Active item selection from inventory **/

            /** Set crosshair to the selected item **/
            crosshairImage.sprite = playerInventory.selectedItem.icon;
            crosshairImage.transform.localScale = Vector3.one * playerInventory.selectedItem.iconScale;

            /** Set crosshair pulsing animation based on player look objects **/
            if (rayHit && hitInfo.transform.CompareTag("Interactable"))
            {
                crosshairAnimator.SetBool("PulseCrosshair", true);
            }
            else
            {
                crosshairAnimator.SetBool("PulseCrosshair", false);
            }
        }
        else
        {
            /** No active item selection from inventory **/

            Crosshair crosshair;
            crosshairAnimator.SetBool("PulseCrosshair", false);

            /** Set crosshair based on player's ability to interact with the object being looked at **/
            if (rayHit && hitInfo.transform.CompareTag("Interactable") && player.CanInteract())
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
