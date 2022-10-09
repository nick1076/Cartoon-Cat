using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeChange : MonoBehaviour
{
    public VolumeProfile newPP;
    public VolumeProfile newSky;

    public Volume PP;
    public Volume Sky;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PP.profile = newPP;
            Sky.profile = newSky;
        }
    }
}
