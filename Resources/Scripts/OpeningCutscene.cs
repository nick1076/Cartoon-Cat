using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class OpeningCutscene : MonoBehaviour
{
    public GameObject carDrivingSound;
    public GameObject carDoorSound;
    public GameObject blackUI;
    public GameObject fadeinUI;
    public GameObject dateText;
    public GameObject controlList;

    public bool cutsceneEnabled;

    public FpsControllerLPFP player;

    public bool inState;

    private void Start()
    {
        if (!cutsceneEnabled)
        {
            GameObject.Find("Task Manager").GetComponent<TaskManager>().AddTask("grab_gear&Grab Your Gear&0&1");
            blackUI.SetActive(false);
            return;
        }
        player.canMove = false;
        player.ambience.enabled = false;
        StartCoroutine(Begin());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inState)
        {
            inState = false;
            controlList.GetComponent<Animator>().SetTrigger("Fade");
            StartCoroutine(Continue());
        }
    }

    IEnumerator Begin()
    {
        yield return new WaitForSeconds(2.0f);
        controlList.SetActive(true);
        player.moveVarLocked = true;
        yield return new WaitForSeconds(1.0f);
        inState = true;
    }

    IEnumerator Continue()
    {
        yield return new WaitForSeconds(6.0f);
        carDrivingSound.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        dateText.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        carDoorSound.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        blackUI.SetActive(false);
        fadeinUI.SetActive(true);
        player.ambience.volume = 0;
        player.ambience.enabled = true;

        player.moveVarLocked = false;
        player.canMove = true;

        yield return new WaitForSeconds(1.5f);

        GameObject.Find("Task Manager").GetComponent<TaskManager>().AddTask("grab_gear&Grab Your Gear&0&1");

        //controlList.SetActive(true);
    }
}
