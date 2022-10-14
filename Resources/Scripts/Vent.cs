using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
    //Hidden Code Variables
    bool open;
    bool moving;
    bool canBeOpened;

    //Locations
    public Transform openPosition;
    public Transform defaultPosition;

    //Effects
    public GameObject openEffect;
    public GameObject closeEffect;

    void Update()
    {
        if (GameObject.Find("Player") == null || defaultPosition == null)
        {
            return;
        }

        float dist = Vector3.Distance(GameObject.Find("Player").transform.position, defaultPosition.position);
        if (dist < 1.5f && dist > 0.3f)
        {
            canBeOpened = true;
        }
        else
        {
            canBeOpened = false;
        }

        if (canBeOpened && Input.GetKeyDown(KeyCode.E) && !moving)
        {
            if (open)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    IEnumerator OpenVent()
    {
        moving = true;

        Instantiate(openEffect, this.transform.position, this.transform.rotation, this.transform);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.00923f, this.transform.position.z);
        }

        moving = false;
        open = true;
    }

    IEnumerator CloseVent()
    {
        moving = true;

        Instantiate(closeEffect, this.transform.position, this.transform.rotation, this.transform);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.00923f, this.transform.position.z);
        }

        moving = false;
        open = false;
    }

    public void Open()
    {
        if (!open)
        {
            StartCoroutine(OpenVent());
        }
    }

    public void Close()
    {
        if (open)
        {
            StartCoroutine(CloseVent());
        }
    }
}
