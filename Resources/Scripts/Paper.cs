using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;
using UnityEngine.Events;
using FPSControllerLPFP;

public class Paper : MonoBehaviour
{

    public bool opened;
    public GameObject creationOnOpen;
    public GameObject itemIndicator;

    public UnityEvent onRead;

    public bool orderedType;
    public List<GameObject> papers = new List<GameObject>();
    public TaskManager taskMan;
    public string task;

    private GameObject openedObj;

    private bool readBefore;

    bool just;

    public bool within;

    public bool turnedOn = true;

    private void Start()
    {
        if (!turnedOn)
        {
            itemIndicator.SetActive(false);
        }
    }

    public void Enable()
    {
        if (itemIndicator != null)
        {
            itemIndicator.SetActive(true);
        }
        turnedOn = true;
    }

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
        if (GameObject.FindWithTag("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>().mouseOut || !turnedOn)
        {
            return;
        }
        if (within)
        {
            if (Input.GetKeyDown(KeyCode.E) && !opened)
            {
                if (!orderedType)
                {
                    just = true;
                    openedObj = Instantiate(creationOnOpen, GameObject.FindWithTag("InspectPos").transform.position, GameObject.FindWithTag("InspectPos").transform.rotation, GameObject.FindWithTag("InspectPos").transform);
                    opened = true;

                    GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().enabled = false;

                    if (itemIndicator != null)
                    {
                        Destroy(itemIndicator);
                    }
                }
                else
                {
                    if (creationOnOpen == null)
                    {
                        //Set
                        foreach (Task taskList in taskMan.tasks)
                        {
                            if (taskList.ID == task)
                            {
                                creationOnOpen = papers[taskList.current];
                                break;
                            }
                        }

                        just = true;
                        openedObj = Instantiate(creationOnOpen, GameObject.FindWithTag("InspectPos").transform.position, GameObject.FindWithTag("InspectPos").transform.rotation, GameObject.FindWithTag("InspectPos").transform);
                        opened = true;

                        GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().enabled = false;

                        if (itemIndicator != null)
                        {
                            Destroy(itemIndicator);
                        }
                    }
                    else
                    {
                        just = true;
                        openedObj = Instantiate(creationOnOpen, GameObject.FindWithTag("InspectPos").transform.position, GameObject.FindWithTag("InspectPos").transform.rotation, GameObject.FindWithTag("InspectPos").transform);
                        opened = true;

                        GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().enabled = false;

                        if (itemIndicator != null)
                        {
                            Destroy(itemIndicator);
                        }
                    }
                }
            }
        }
        if (opened)
        {
            GameObject.FindWithTag("Player").GetComponent<FpsControllerLPFP>().canMove = false;
        }
        if (!just)
        {
            if (Input.GetKeyDown(KeyCode.E) && opened)
            {
                opened = false;

                GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().enabled = true;

                GameObject.FindWithTag("Player").GetComponent<FpsControllerLPFP>().canMove = true;

                if (creationOnOpen != null)
                {
                    Destroy(openedObj);
                }

                if (!readBefore)
                {
                    onRead.Invoke();
                    readBefore = true;
                }
            }
        }
        else
        {
            just = false;
        }
    }
}
