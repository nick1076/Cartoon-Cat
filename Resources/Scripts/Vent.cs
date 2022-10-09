using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{

    public bool open;
    public Transform openPosition;
    public bool hovered;
    public bool moving;

    public GameObject openSound;
    public GameObject closeSound;

    private void OnTriggerEnter(Collider other)
    {
        hovered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        hovered = false;
    }

    void Update()
    {
        if (hovered && Input.GetKeyDown(KeyCode.E) && !moving)
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

        Instantiate(openSound, this.transform.position, this.transform.rotation, this.transform);

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

        Instantiate(closeSound, this.transform.position, this.transform.rotation, this.transform);

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
