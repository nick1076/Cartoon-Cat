using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotableItem : MonoBehaviour
{
    public GameObject cameraIcon;
    public UnityEvent eventOnCapture;

    public bool on = true;

    public void OnPhotoCaptured()
    {
        if (!on)
        {
            return;
        }

        eventOnCapture.Invoke();
        cameraIcon.SetActive(false);
        on = false;
    }


    public void ReActivate()
    {
        on = true;
        cameraIcon.SetActive(true);
    }
}
