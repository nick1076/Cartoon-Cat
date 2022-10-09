using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class Flashlight : MonoBehaviour
{
    public GameObject light;
    public GameObject clicksound;

    public FPSControllerLPFP.FpsControllerLPFP player;

    void Update()
    {
        if (!player.canMove)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (light.activeSelf)
            {
                light.SetActive(false);
            }
            else
            {
                light.SetActive(true);
            }

            Instantiate(clicksound);
        }
    }
}
