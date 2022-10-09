using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boards : MonoBehaviour
{
    public bool readyToBeBroken;
    [TextArea]
    public string notReadyDialouge;
    [TextArea]
    public string noHammerDialouge;
    [TextArea]
    public string hammerHintDialouge;

    public GameObject breakSound;

    bool within;

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
        if (within)
        {
            if (!GameObject.Find("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>().canMove)
            {
                GameObject.Find("Player").GetComponent<PlayerInventory>().hammerObj.GetComponent<Hammer>().canSwing = false;
                return;
            }
            if (GameObject.Find("Player").GetComponent<PlayerInventory>().hasHammer)
            {
                if (GameObject.Find("Player").GetComponent<PlayerInventory>().selectedItem == "Hammer")
                {
                    if (readyToBeBroken)
                    {
                        GameObject.Find("Player").GetComponent<PlayerInventory>().hammerObj.GetComponent<Hammer>().canSwing = true;
                    }
                    else
                    {
                        GameObject.Find("Player").GetComponent<PlayerInventory>().hammerObj.GetComponent<Hammer>().canSwing = false;
                    }
                }
                else
                {
                    GameObject.Find("Player").GetComponent<PlayerInventory>().hammerObj.GetComponent<Hammer>().canSwing = false;
                }
            }
            else
            {

                GameObject.Find("Player").GetComponent<PlayerInventory>().hammerObj.GetComponent<Hammer>().canSwing = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E))
            {
                if (!GameObject.Find("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>().canMove)
                {
                    return;
                }
                if (GameObject.Find("Player").GetComponent<PlayerInventory>().hasHammer)
                {
                    if (GameObject.Find("Player").GetComponent<PlayerInventory>().selectedItem == "Hammer")
                    {
                        if (readyToBeBroken)
                        {
                            //Break Boards
                            Instantiate(breakSound, this.transform.position, this.transform.rotation);
                            Destroy(this.gameObject);
                        }
                        else
                        {
                            //Use notReadyDialouge
                            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge(notReadyDialouge);
                        }
                    }
                    else
                    {
                        if (readyToBeBroken)
                        {
                            //Use hammerHintDialouge
                            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge(hammerHintDialouge);
                        }
                        else
                        {
                            //Use notReadyDialouge
                            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge(notReadyDialouge);
                        }
                    }
                }
                else
                {
                    //Use noHammerDialouge
                    GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge(noHammerDialouge);
                }
            }
        }
    }

    public void AlterReadyness(bool type)
    {
        readyToBeBroken = type;
    }
}
