using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class AmbienceZone : MonoBehaviour
{

    public FPSControllerLPFP.FpsControllerLPFP player;
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.SetAmbience(id);
        }
    }
}
