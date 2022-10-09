using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class CameraItem : MonoBehaviour
{
    public GameObject flash;
    public GameObject flashPoint;
    public GameObject clicksound;

    public GameObject currentFlash;

    public FPSControllerLPFP.FpsControllerLPFP player;

    void Update()
    {
        if (!player.canMove)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentFlash != null)
            {
                Destroy(currentFlash.gameObject);
            }

            Instantiate(clicksound);
            currentFlash = Instantiate(flash, flashPoint.transform.position, flashPoint.transform.rotation, flashPoint.transform);
        }
    }
}
