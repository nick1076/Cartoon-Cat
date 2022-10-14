using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{

    public Transform resetPos;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = resetPos.position;
            collision.transform.rotation = resetPos.rotation;
            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge("You clipped outside the map so I reset you : )", true);
        }
    }
}
