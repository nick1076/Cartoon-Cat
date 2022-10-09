using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{

    public int item;
    public GameObject objOnGrab;

    public UnityEvent onGrab;

    public bool within;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            within = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            within = false;
        }
    }

    private void Update()
    {
        if (within && Input.GetKeyDown(KeyCode.E))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().Item(item);
            if (objOnGrab != null)
            {
                Instantiate(objOnGrab);
            }

            onGrab.Invoke();
            Destroy(this.gameObject);
        }
    }
}
