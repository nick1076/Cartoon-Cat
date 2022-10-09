using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public FPSControllerLPFP.FpsControllerLPFP player;
    public bool canSwing;

    void Update()
    {
        if (!player.canMove)
        {
            return;
        }
        if (canSwing)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E))
            {
                this.gameObject.GetComponent<Animator>().SetTrigger("Swing");
            }
        }
    }
}
