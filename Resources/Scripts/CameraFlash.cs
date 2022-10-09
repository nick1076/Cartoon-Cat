using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlash : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Notable")
        {
            other.gameObject.GetComponent<NotableItem>().OnPhotoCaptured();
        }
    }
}
