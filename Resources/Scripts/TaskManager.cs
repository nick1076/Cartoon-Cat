using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TaskManager : MonoBehaviour
{
    public GameObject defaultTaskListing;
    public GameObject taskList;
    public GameObject taskCanvas;

    public string executedTask;

    public GameObject completeEffect;
    public GameObject givenEffect;

    public List<Task> tasks = new List<Task>();

    public GameObject cartoonCat;
    public GameObject playerRespawn;
    public GameObject mazePost;

    public GameObject newTaskFX;

    public float globalTaskDelay = 5.0f;

    public List<string> taskQue = new List<string>();
    bool runningTask;

    private void Start()
    {
        StartCoroutine(RunTaskQue());
    }

    public void AddTask(string data)
    {
        foreach (Task task in tasks)
        {
            if (task.raw == data)
            {
                return;
            }
        }

        taskQue.Add(data);
    }

    IEnumerator RunTaskQue()
    {
        yield return new WaitForSeconds(0.1f);

        if (!runningTask && taskQue.Count >= 1)
        {
            runningTask = true;
            string[] dat = taskQue[0].Split('&');
            Task task = new Task
            {
                displayName = dat[1],
                ID = dat[0],
                current = (int)Int64.Parse(dat[2]),
                max = (int)Int64.Parse(dat[3]),
                raw = taskQue[0]
            };

            taskQue.Remove(taskQue[0]);

            TaskDisplay taskDisp = Instantiate(newTaskFX, taskCanvas.transform).GetComponent<TaskDisplay>();
            taskDisp.Initialize(task.displayName);

            StartCoroutine(delayAdd(task));
            yield return new WaitForSeconds(7.0f);
            runningTask = false;
        }

        StartCoroutine(RunTaskQue());
    }

    IEnumerator delayAdd(Task task)
    {
        yield return new WaitForSeconds(globalTaskDelay);

        executedTask = task.ID;

        task.listing = Instantiate(defaultTaskListing, taskList.transform);

        if (task.max > 1)
        {
            task.listing.GetComponent<TextMeshProUGUI>().text = task.displayName + " (" + task.current + "/" + task.max + ")";
        }
        else
        {
            task.listing.GetComponent<TextMeshProUGUI>().text = task.displayName;
        }

        tasks.Add(task);
        Instantiate(givenEffect);
    }

    IEnumerator EventMaze()
    {
        GameObject.Find("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>().SetAmbience(-2);
        yield return new WaitForSeconds(3.0f);
        GameObject.Find("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>().SetAmbience(5) ;
        GameObject.Find("Maze Pre").SetActive(false);
        mazePost.SetActive(true);
        //playerRespawn.transform.position = GameObject.FindWithTag("Player").transform.position;
        //playerRespawn.transform.rotation = GameObject.FindWithTag("Player").transform.rotation;
        yield return new WaitForSeconds(1.0f);
        AddTask("welcome&Welcome to my world&0&1");
        yield return new WaitForSeconds(8.0f);
        GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge("What the hell is this!?");
        GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge("I NEED more photos...");
        yield return new WaitForSeconds(8.0f);
        AddTask("photos3&Take photos of the drawings&0&3");
        yield return new WaitForSeconds(3.0f);
        cartoonCat.SetActive(true);
        GameObject.Find("Cartoon Cat").GetComponent<CartoonCat>().SetCatPositionToFurthest(GameObject.FindWithTag("Player").transform);
    }

    public void ResetTask(string id)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].ID == id)
            {
                tasks[i].current = 0;

                tasks[i].listing.GetComponent<TextMeshProUGUI>().text = tasks[i].displayName + " (" + tasks[i].current + "/" + tasks[i].max + ")";
            }
        }
    }

    public void CompleteTask(string id)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].ID == id)
            {
                tasks[i].current += 1;

                if (tasks[i].current >= tasks[i].max)
                {
                    Instantiate(completeEffect);
                    Destroy(tasks[i].listing);
                    tasks.Remove(tasks[i]);

                    if (id == "take_photos_2" || id == "read_papers")
                    {
                        bool canProceed = true;
                        foreach (Task taskCurrent in tasks)
                        {
                            if (taskCurrent.ID == "take_photos_2" || taskCurrent.ID == "read_papers")
                            {
                                canProceed = false;
                            }
                        }

                        if (canProceed)
                        {
                            StartCoroutine(EventMaze());
                        }
                    }
                    else if (id == "photos3")
                    {
                        StartCoroutine(EventPhotoCat());
                    }
                    return;
                }
                else
                {
                    tasks[i].listing.GetComponent<TextMeshProUGUI>().text = tasks[i].displayName + " (" + tasks[i].current + "/" + tasks[i].max + ")";
                }
            }
        }
    }

    IEnumerator EventPhotoCat()
    {
        GameObject.FindWithTag("Cartoon Cat").GetComponent<CartoonCat>().level = 1;
        yield return new WaitForSeconds(3.0f);
        GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge("What if I got a photo of the cat...");
        GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().QueDialouge("Then everyone would belive me...");
        yield return new WaitForSeconds(8.0f);
        AddTask("photocat&Take a photo of the Cat&0&1");
    }

    public void RemoveTask(string id)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].ID == id)
            {
                Destroy(tasks[i].listing);
                tasks.Remove(tasks[i]);
                return;
            }
        }
    }
}
