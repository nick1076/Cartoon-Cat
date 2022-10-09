using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class PlayerInventory : MonoBehaviour
{

    public bool hasFlashlight;
    public bool hasCamera;
    public bool hasHammer;

    public GameObject flashlightObj;
    public GameObject cameraObj;
    public GameObject hammerObj;

    public FpsControllerLPFP player;

    public string selectedItem = "null";

    private void Start()
    {
        flashlightObj.SetActive(false);
        cameraObj.SetActive(false);
        hammerObj.SetActive(false);

        if (hasFlashlight)
        {
            flashlightObj.SetActive(true);
        }
    }

    private void Update()
    {
        if (!player.canMove)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hasFlashlight)
            {
                selectedItem = "Flashlight";
                flashlightObj.SetActive(true);
                cameraObj.SetActive(false);
                hammerObj.SetActive(false);

                if (cameraObj.GetComponent<CameraItem>().currentFlash != null)
                {
                    Destroy(cameraObj.GetComponent<CameraItem>().currentFlash.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hasCamera)
            {
                selectedItem = "Camera";
                flashlightObj.SetActive(false);
                cameraObj.SetActive(true);
                hammerObj.SetActive(false);

                if (cameraObj.GetComponent<CameraItem>().currentFlash != null)
                {
                    Destroy(cameraObj.GetComponent<CameraItem>().currentFlash.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasHammer)
            {
                selectedItem = "Hammer";
                flashlightObj.SetActive(false);
                cameraObj.SetActive(false);
                hammerObj.SetActive(true);

                if (cameraObj.GetComponent<CameraItem>().currentFlash != null)
                {
                    Destroy(cameraObj.GetComponent<CameraItem>().currentFlash.gameObject);
                }
            }
        }
    }

    public void Item(int val)
    {
        if (val == -1)
        {
            hasFlashlight = true;
            hasCamera = true;
            flashlightObj.SetActive(true);
            cameraObj.SetActive(false);
        }
        else if (val == 0)
        {
            hasFlashlight = true;
            flashlightObj.SetActive(true);
            cameraObj.SetActive(false);
        }
        if (val == 1)
        {
            hasCamera = true;
            flashlightObj.SetActive(false);
            cameraObj.SetActive(true);
        }
        if (val == 2)
        {
            hasHammer = true;
            flashlightObj.SetActive(false);
            cameraObj.SetActive(false);
            hammerObj.SetActive(true);
        }

        if (cameraObj.GetComponent<CameraItem>().currentFlash != null)
        {
            Destroy(cameraObj.GetComponent<CameraItem>().currentFlash.gameObject);
        }
    }

}
